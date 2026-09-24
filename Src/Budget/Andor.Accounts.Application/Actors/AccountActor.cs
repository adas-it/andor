using Akka.Actor;
using Andor.Accounts.Application.Commands.Contracts;
using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Accounts.Domain.Accounts.Repositories;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories.Repositories;
using Andor.Accounts.Domain.Currencies.Repositories;
using Andor.Accounts.Domain.FinancialMovements;
using Andor.Accounts.Domain.PaymentMethods;
using Andor.Accounts.Domain.PaymentMethods.Repositories;
using Andor.Accounts.Domain.SubCategories.Repositories;
using Andor.Foundation.Domain.ValuesObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Accounts.Application.Actors;

public class AccountActor : ReceiveActor, IWithUnboundedStash
{
    private readonly AccountId _id;
    private Account? _account;
    private readonly IServiceProvider _serviceProvider;

    public IStash? Stash { get; set; }

    public AccountActor(AccountId id, IServiceProvider serviceProvider)
    {
        _id = id;
        _serviceProvider = serviceProvider;

        Become(Loading);
    }

    protected override void PreStart()
    {
        Self.Tell(new PreLoadAccount(_id));
        base.PreStart();
    }

    private void Loading()
    {
        ReceiveAsync<PreLoadAccount>(async _ =>
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();

            var result = await repo.GetByIdAsync(_id, CancellationToken.None);

            if (result == null)
                return;

            _account = result;

            Become(Ready);
            Stash!.UnstashAll();
        });

        ReceiveAsync<CreateAccountCommand>(HandleCreateAsync);

        ReceiveAny(_ => Stash!.Stash());
    }

    private void Ready()
    {
        ReceiveAsync<SeedAccountDefaultsCommand>(HandleSeedDefaultsAsync);
        ReceiveAsync<AddFinancialMovementCommand>(HandleAddFinancialMovementAsync);
        ReceiveAsync<EditFinancialMovementCommand>(HandleEditFinancialMovementAsync);
        ReceiveAsync<DeleteFinancialMovementCommand>(HandleDeleteFinancialMovementAsync);
        ReceiveAsync<InviteMemberByEmailCommand>(HandleInviteMemberByEmailAsync);
        ReceiveAsync<InviteMemberByUserCommand>(HandleInviteMemberByUserAsync);
        ReceiveAsync<AnswerInviteCommand>(HandleAnswerInviteAsync);
        ReceiveAsync<CreateCustomCategoryCommand>(HandleCreateCustomCategoryAsync);
        ReceiveAsync<CreateCustomSubCategoryCommand>(HandleCreateCustomSubCategoryAsync);
        ReceiveAsync<CreateCustomPaymentMethodCommand>(HandleCreateCustomPaymentMethodAsync);
        ReceiveAsync<UpdateAccountDetailsCommand>(HandleUpdateAccountDetailsAsync);
    }


    private async Task HandleUpdateAccountDetailsAsync(UpdateAccountDetailsCommand cmd)
    {
        using var scope = _serviceProvider.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();
        var currencyRepo = scope.ServiceProvider.GetRequiredService<ICommandsCurrencyRepository>();
        var validator = scope.ServiceProvider.GetRequiredService<IAccountValidator>();

        var currency = await currencyRepo.GetByIdAsync(cmd.CurrencyId, cmd.CancellationToken);

        if (currency == null)
        {
            var notFound = DomainResult.Failure(errors: new List<Notification>
            {
                new(nameof(cmd.CurrencyId), "Currency not found.", AccountErrorCode.CurrencyNotFound),
            });

            Sender.Tell((notFound, (Account?)null));
            return;
        }

        var domainResult = await _account!.UpdateAccountDetailsAsync(
            cmd.Name,
            cmd.Description,
            currency,
            cmd.CurrentUser.UserId,
            validator,
            cmd.CancellationToken);

        if (_account?.Events.Count > 0)
            await repo.PersistAsync(_account, cmd.CancellationToken);

        Sender.Tell((domainResult, _account));
    }

    private async Task HandleCreateAsync(CreateAccountCommand cmd)
    {
        using var scope = _serviceProvider.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();
        var currencyRepo = scope.ServiceProvider.GetRequiredService<ICommandsCurrencyRepository>();
        var validator = scope.ServiceProvider.GetRequiredService<IAccountValidator>();

        var currency = await currencyRepo.GetByIdAsync(cmd.CurrencyId, cmd.CancellationToken);

        if (currency == null)
        {
            var notFound = DomainResult.Failure(errors: new List<Notification>
            {
                new(nameof(cmd.CurrencyId), "Currency not found.", AccountErrorCode.CurrencyNotFound),
            });

            Sender.Tell((notFound, (Account?)null));
            return;
        }

        var (domainResult, account) = await Account.NewAsync(
            _id,
            cmd.Name,
            cmd.Description,
            currency,
            cmd.CurrentUser.UserId,
            validator,
            cmd.CancellationToken);

        if (account?.Events.Count > 0)
            await repo.PersistAsync(account, cmd.CancellationToken);

        _account = account;
        Sender.Tell((domainResult, account));

        if (account != null)
        {
            Become(Ready);
            Stash!.UnstashAll();
        }
    }

    private async Task HandleSeedDefaultsAsync(SeedAccountDefaultsCommand cmd)
    {
        using var scope = _serviceProvider.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();
        var categoryRepo = scope.ServiceProvider.GetRequiredService<ICommandsCategoryRepository>();
        var paymentMethodRepo = scope.ServiceProvider.GetRequiredService<ICommandsPaymentMethodRepository>();
        var subCategoryRepo = scope.ServiceProvider.GetRequiredService<ICommandsSubCategoryRepository>();

        var categories = await categoryRepo.GetTemplatesAsync(cmd.CancellationToken);
        var paymentMethods = await paymentMethodRepo.GetTemplatesAsync(cmd.CancellationToken);
        var subCategories = await subCategoryRepo.GetTemplatesAsync(cmd.CancellationToken);

        var result = DomainResult.Success();

        // Categories and payment methods must be attached before subcategories: Account.AddTemplateSubCategory
        // validates that both the subcategory's category and its default payment method already belong to the account.
        foreach (var category in categories)
        {
            result = _account!.AddTemplateCategory(category, cmd.CurrentUser.UserId);

            if (result.IsFailure)
                break;
        }

        if (result.IsSuccess)
        {
            foreach (var paymentMethod in paymentMethods)
            {
                result = _account!.AddTemplatePaymentMethod(paymentMethod, cmd.CurrentUser.UserId);

                if (result.IsFailure)
                    break;
            }
        }

        if (result.IsSuccess)
        {
            foreach (var subCategory in subCategories)
            {
                result = _account!.AddTemplateSubCategory(subCategory, cmd.CurrentUser.UserId);

                if (result.IsFailure)
                    break;
            }
        }

        if (result.IsSuccess && _account?.Events.Count > 0)
            await repo.PersistAsync(_account, cmd.CancellationToken);

        Sender.Tell((result, _account));
    }

    private async Task HandleAddFinancialMovementAsync(AddFinancialMovementCommand cmd)
    {
        // Reuse the SubCategory/PaymentMethod instances already hanging off _account (loaded via
        // AutoInclude on AccountSubCategory.SubCategory / AccountPaymentMethod.PaymentMethod)
        // instead of fetching separate copies. AddFinancialMovement below requires them to belong
        // to the account anyway, and fetching separate copies gives EF two different tracked
        // instances for the same row later, which blows up on save ("cannot track twice").
        var accountSubCategory = _account!.SubCategories.FirstOrDefault(x => x.SubCategoryId == cmd.SubCategoryId);
        var accountPaymentMethod = _account.PaymentMethods.FirstOrDefault(x => x.PaymentMethodId == cmd.PaymentMethodId);

        if (accountSubCategory == null || accountPaymentMethod == null)
        {
            var errors = new List<Notification>();

            if (accountSubCategory == null)
                errors.Add(new(nameof(cmd.SubCategoryId), "SubCategory not found.", AccountErrorCode.FinancialMovementSubCategoryNotFound));

            if (accountPaymentMethod == null)
                errors.Add(new(nameof(cmd.PaymentMethodId), "PaymentMethod not found.", AccountErrorCode.FinancialMovementPaymentMethodNotFound));

            Sender.Tell((DomainResult.Failure(errors: errors), _account));
            return;
        }

        var (movementResult, movement) = FinancialMovement.New(
            cmd.Date,
            cmd.Description,
            accountSubCategory.SubCategory,
            accountPaymentMethod.PaymentMethod,
            _account,
            cmd.Value,
            cmd.Status);

        if (movement == null)
        {
            Sender.Tell((movementResult, _account));
            return;
        }

        var result = _account!.AddFinancialMovement(movement, cmd.CurrentUser.UserId);

        if (result.IsSuccess && _account.Events.Count > 0)
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();

            await repo.UpsertFinancialMovement(movement, cmd.CancellationToken);
            await repo.PersistAsync(_account, cmd.CancellationToken);
        }

        Sender.Tell((result, _account));
    }

    private async Task HandleEditFinancialMovementAsync(EditFinancialMovementCommand cmd)
    {
        using var scope = _serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();

        var movement = await repo.GetFinancialMovementByIdAsync(cmd.FinancialMovementId, cmd.CancellationToken);

        if (movement == null || movement.AccountId != _account!.Id)
        {
            var notFound = DomainResult.Failure(errors: new List<Notification>
            {
                new(nameof(cmd.FinancialMovementId), "Financial movement not found.", AccountErrorCode.FinancialMovementNotFound),
            });

            Sender.Tell((notFound, (FinancialMovement?)null));
            return;
        }

        // Same lookup as HandleAddFinancialMovementAsync: reuse the SubCategory/PaymentMethod
        // instances already hanging off _account instead of fetching separate tracked copies.
        var accountSubCategory = _account.SubCategories.FirstOrDefault(x => x.SubCategoryId == cmd.SubCategoryId);
        var accountPaymentMethod = _account.PaymentMethods.FirstOrDefault(x => x.PaymentMethodId == cmd.PaymentMethodId);

        if (accountSubCategory == null || accountPaymentMethod == null)
        {
            var errors = new List<Notification>();

            if (accountSubCategory == null)
                errors.Add(new(nameof(cmd.SubCategoryId), "SubCategory not found.", AccountErrorCode.FinancialMovementSubCategoryNotFound));

            if (accountPaymentMethod == null)
                errors.Add(new(nameof(cmd.PaymentMethodId), "PaymentMethod not found.", AccountErrorCode.FinancialMovementPaymentMethodNotFound));

            Sender.Tell((DomainResult.Failure(errors: errors), (FinancialMovement?)null));
            return;
        }

        var monthChanged = movement.Date.Year != cmd.Date.Year || movement.Date.Month != cmd.Date.Month;
        var typeChanged = movement.Type != accountSubCategory.SubCategory.Type;

        DomainResult result;
        FinancialMovement? output;

        if (monthChanged || typeChanged)
        {
            // The CashFlow projection buckets by account/month/type, so a movement that crosses
            // either boundary can't be adjusted in place: pull it out of its current bucket
            // (soft-delete + Removed event) and drop a brand new movement into the new one
            // (Added event), instead of trying to move value between two different rows.
            result = _account.RemoveFinancialMovement(movement, cmd.CurrentUser.UserId);

            if (result.IsFailure)
            {
                Sender.Tell((result, (FinancialMovement?)null));
                return;
            }

            _ = movement.SoftDelete();

            var (createResult, newMovement) = FinancialMovement.New(
                cmd.Date,
                cmd.Description,
                accountSubCategory.SubCategory,
                accountPaymentMethod.PaymentMethod,
                _account,
                cmd.Value,
                cmd.Status);

            if (newMovement == null)
            {
                Sender.Tell((createResult, (FinancialMovement?)null));
                return;
            }

            result = _account.AddFinancialMovement(newMovement, cmd.CurrentUser.UserId);

            if (result.IsSuccess)
            {
                await repo.UpsertFinancialMovement(movement, cmd.CancellationToken);
                await repo.UpsertFinancialMovement(newMovement, cmd.CancellationToken);
                await repo.PersistAsync(_account, cmd.CancellationToken);
            }

            output = result.IsSuccess ? newMovement : null;
        }
        else
        {
            var previousValue = movement.Value;
            var previousStatus = movement.Status;

            result = movement.Edit(
                cmd.Date,
                cmd.Description,
                accountSubCategory.SubCategory,
                accountPaymentMethod.PaymentMethod,
                cmd.Value,
                cmd.Status);

            if (result.IsSuccess)
            {
                result = _account.EditFinancialMovement(movement, previousValue, previousStatus, cmd.CurrentUser.UserId);
            }

            if (result.IsSuccess)
            {
                try
                {
                    await repo.UpsertFinancialMovement(movement, cmd.CancellationToken);
                    await repo.PersistAsync(_account, cmd.CancellationToken);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            output = result.IsSuccess ? movement : null;
        }

        Sender.Tell((result, output));
    }

    private async Task HandleDeleteFinancialMovementAsync(DeleteFinancialMovementCommand cmd)
    {
        using var scope = _serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();

        var movement = await repo.GetFinancialMovementByIdAsync(cmd.FinancialMovementId, cmd.CancellationToken);

        if (movement == null || movement.AccountId != _account!.Id)
        {
            var notFound = DomainResult.Failure(errors: new List<Notification>
            {
                new(nameof(cmd.FinancialMovementId), "Financial movement not found.", AccountErrorCode.FinancialMovementNotFound),
            });

            Sender.Tell((notFound, (FinancialMovement?)null));
            return;
        }

        var result = _account.RemoveFinancialMovement(movement, cmd.CurrentUser.UserId);

        if (result.IsSuccess)
        {
            await repo.UpsertFinancialMovement(movement, cmd.CancellationToken);
            await repo.PersistAsync(_account, cmd.CancellationToken);
        }

        Sender.Tell((result, result.IsSuccess ? movement : null));
    }

    private async Task HandleInviteMemberByEmailAsync(InviteMemberByEmailCommand cmd)
    {
        var result = _account!.InviteMemberByEmail(cmd.Email, cmd.Permission, cmd.CurrentUser.UserId);

        if (result.IsSuccess && _account.Events.Count > 0)
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();
            await repo.PersistAsync(_account, cmd.CancellationToken);
        }

        Sender.Tell((result, _account));
    }

    private async Task HandleInviteMemberByUserAsync(InviteMemberByUserCommand cmd)
    {
        var result = _account!.InviteMemberByUser(cmd.InvitedUserId, cmd.Permission, cmd.CurrentUser.UserId);

        if (result.IsSuccess && _account.Events.Count > 0)
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();
            await repo.PersistAsync(_account, cmd.CancellationToken);
        }

        Sender.Tell((result, _account));
    }

    private async Task HandleAnswerInviteAsync(AnswerInviteCommand cmd)
    {
        var result = cmd.Accept
            ? _account!.RespondInvite(cmd.InviteId, cmd.CurrentUser.UserId)
            : _account!.RejectInvite(cmd.InviteId, cmd.CurrentUser.UserId);

        if (result.IsSuccess && _account.Events.Count > 0)
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();
            await repo.PersistAsync(_account, cmd.CancellationToken);
        }

        Sender.Tell((result, _account));
    }

    private async Task HandleCreateCustomCategoryAsync(CreateCustomCategoryCommand cmd)
    {
        var result = _account!.CreateCustomCategory(cmd.Name, cmd.Description, cmd.Type, cmd.CurrentUser.UserId);

        if (result.IsSuccess && _account.Events.Count > 0)
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();
            await repo.PersistAsync(_account, cmd.CancellationToken);
        }

        Sender.Tell((result, _account));
    }

    private async Task HandleCreateCustomPaymentMethodAsync(CreateCustomPaymentMethodCommand cmd)
    {
        var result = _account!.CreateCustomPaymentMethod(cmd.Name, cmd.Description, cmd.Type, cmd.CurrentUser.UserId);

        if (result.IsSuccess && _account.Events.Count > 0)
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();
            await repo.PersistAsync(_account, cmd.CancellationToken);
        }

        Sender.Tell((result, _account));
    }

    private async Task HandleCreateCustomSubCategoryAsync(CreateCustomSubCategoryCommand cmd)
    {
        // Resolve the Category/PaymentMethod instances already hanging off _account (same
        // reasoning as HandleAddFinancialMovementAsync) instead of fetching separate copies -
        // Account.CreateCustomSubCategory needs the real domain objects, not just their ids.
        var accountCategory = _account!.Categories.FirstOrDefault(x => x.CategoryId == cmd.CategoryId);

        if (accountCategory == null)
        {
            var notFound = DomainResult.Failure(errors: new List<Notification>
            {
                new(nameof(cmd.CategoryId), "Category not found.", AccountErrorCode.SubCategoryCategoryNotFound),
            });

            Sender.Tell((notFound, _account));
            return;
        }

        PaymentMethod? defaultPaymentMethod = null;

        if (cmd.DefaultPaymentMethodId.HasValue)
        {
            var accountPaymentMethod = _account.PaymentMethods.FirstOrDefault(x => x.PaymentMethodId == cmd.DefaultPaymentMethodId.Value);

            if (accountPaymentMethod == null)
            {
                var notFound = DomainResult.Failure(errors: new List<Notification>
                {
                    new(nameof(cmd.DefaultPaymentMethodId), "Default payment method not found.", AccountErrorCode.SubCategoryDefaultPaymentMethodNotFound),
                });

                Sender.Tell((notFound, _account));
                return;
            }

            defaultPaymentMethod = accountPaymentMethod.PaymentMethod;
        }

        var result = _account.CreateCustomSubCategory(cmd.Name, cmd.Description, accountCategory.Category, defaultPaymentMethod, cmd.CurrentUser.UserId);

        if (result.IsSuccess && _account.Events.Count > 0)
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();
            await repo.PersistAsync(_account, cmd.CancellationToken);
        }

        Sender.Tell((result, _account));
    }

    private record PreLoadAccount(AccountId Id);
}
