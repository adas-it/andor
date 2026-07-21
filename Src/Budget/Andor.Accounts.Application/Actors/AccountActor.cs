using Akka.Actor;
using Andor.Accounts.Application.Commands;
using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Accounts.Domain.Accounts.Repositories;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories.Repositories;
using Andor.Accounts.Domain.Currencies.Repositories;
using Andor.Accounts.Domain.FinancialMovements;
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
        using var scope = _serviceProvider.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<ICommandsAccountRepository>();
        var subCategoryRepo = scope.ServiceProvider.GetRequiredService<ICommandsSubCategoryRepository>();
        var paymentMethodRepo = scope.ServiceProvider.GetRequiredService<ICommandsPaymentMethodRepository>();

        var subCategory = await subCategoryRepo.GetByIdAsync(cmd.SubCategoryId, cmd.CancellationToken);
        var paymentMethod = await paymentMethodRepo.GetByIdAsync(cmd.PaymentMethodId, cmd.CancellationToken);

        if (subCategory == null || paymentMethod == null)
        {
            var errors = new List<Notification>();

            if (subCategory == null)
                errors.Add(new(nameof(cmd.SubCategoryId), "SubCategory not found.", AccountErrorCode.FinancialMovementSubCategoryNotFound));

            if (paymentMethod == null)
                errors.Add(new(nameof(cmd.PaymentMethodId), "PaymentMethod not found.", AccountErrorCode.FinancialMovementPaymentMethodNotFound));

            Sender.Tell((DomainResult.Failure(errors: errors), _account));
            return;
        }

        var (movementResult, movement) = FinancialMovement.New(
            cmd.Date,
            cmd.Description,
            subCategory,
            paymentMethod,
            _account!,
            cmd.Value,
            cmd.Status);

        if (movement == null)
        {
            Sender.Tell((movementResult, _account));
            return;
        }

        var result = _account!.AddFinancialMovement(movement, cmd.CurrentUser.UserId);

        if (result.IsSuccess && _account.Events.Count > 0)
            await repo.PersistAsync(_account, cmd.CancellationToken);

        Sender.Tell((result, _account));
    }

    private record PreLoadAccount(AccountId Id);
}
