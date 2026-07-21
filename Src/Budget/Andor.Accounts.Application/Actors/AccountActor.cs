using Akka.Actor;
using Andor.Accounts.Application.Commands;
using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Accounts.Domain.Accounts.Repositories;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies.Repositories;
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

    private record PreLoadAccount(AccountId Id);
}
