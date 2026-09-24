using Akka.Actor;
using Akka.Hosting;
using Andor.Accounts.Application.Actors;
using Andor.Accounts.Application.Commands.Contracts;
using Andor.Accounts.Application.Commands.Interfaces;
using Andor.Accounts.Contracts.Accounts.Responses;
using Andor.Accounts.Contracts.FinancialMovements.Response;
using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.Repositories;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Application.Commands;

public class AccountCommandsService(ActorRegistry registry, ICommandsAccountRepository repository) : IAccountCommandsService
{
    private readonly IActorRef _accountActor = registry.Get<AccountManagerActor>();

    public Task<ApplicationResult<AccountOutput?>> CreateAccountAsync(CreateAccountCommand command)
        => Handler(command);

    public Task<ApplicationResult<AccountOutput?>> SeedAccountDefaultsAsync(SeedAccountDefaultsCommand command)
        => Handler(command);

    public Task<ApplicationResult<AccountOutput?>> AddFinancialMovementAsync(AddFinancialMovementCommand command)
        => Handler(command);

    public Task<ApplicationResult<FinancialMovementOutput?>> EditFinancialMovementAsync(EditFinancialMovementCommand command)
        => Handler<FinancialMovement, FinancialMovementOutput?>(command, x => x.ToFinancialMovementOutput());

    public Task<ApplicationResult<FinancialMovementOutput?>> DeleteFinancialMovementAsync(DeleteFinancialMovementCommand command)
        => Handler<FinancialMovement, FinancialMovementOutput?>(command, x => x.ToFinancialMovementOutput());

    public Task<ApplicationResult<AccountOutput?>> InviteMemberByEmailAsync(InviteMemberByEmailCommand command)
        => Handler(command);

    public Task<ApplicationResult<AccountOutput?>> InviteMemberByUserAsync(InviteMemberByUserCommand command)
        => Handler(command);

    public Task<ApplicationResult<AccountOutput?>> AnswerInviteAsync(AnswerInviteCommand command)
        => Handler(command);

    public Task<ApplicationResult<AccountOutput?>> CreateCustomCategoryAsync(CreateCustomCategoryCommand command)
        => Handler(command);

    public Task<ApplicationResult<AccountOutput?>> CreateCustomSubCategoryAsync(CreateCustomSubCategoryCommand command)
        => Handler(command);

    public Task<ApplicationResult<AccountOutput?>> CreateCustomPaymentMethodAsync(CreateCustomPaymentMethodCommand command)
        => Handler(command);

    public Task<ApplicationResult<AccountOutput?>> UpdateAccountDetailsAsync(UpdateAccountDetailsCommand command)
        => Handler(command);

    public async Task<ApplicationResult<AccountOutput?>> GetByIdAsync(AccountId id, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<AccountOutput?>.Success();

        var account = await repository.GetByIdAsync(id, cancellationToken);

        if (account != null)
        {
            _ = response.SetData(account.ToAccountOutput());
        }

        return response;
    }

    private Task<ApplicationResult<AccountOutput?>> Handler(ICommands<AccountId> command)
        => Handler<Account, AccountOutput?>(command, x => x.ToAccountOutput());

    private async Task<ApplicationResult<TResponse>> Handler<TResult, TResponse>(
        ICommands<AccountId> command, Func<TResult, TResponse> mapper)
        where TResult : class?
        where TResponse : class?
    {
        var response = ApplicationResult<TResponse>.Success();

        var (result, account) = await _accountActor.Ask<(DomainResult, TResult)>(command,
            command.CancellationToken);

        if (result.IsFailure)
        {
            await HandleAccountResult.HandleResultAccount(result, response);
            return response;
        }

        if (account != null)
        {
            _ = response.SetData(mapper(account));
        }

        return response;
    }
}
