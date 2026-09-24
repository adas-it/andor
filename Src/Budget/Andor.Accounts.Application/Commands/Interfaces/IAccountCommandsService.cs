using Andor.Accounts.Application.Commands.Contracts;
using Andor.Accounts.Contracts.Accounts.Responses;
using Andor.Accounts.Contracts.FinancialMovements.Response;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Application.Commands.Interfaces;

public interface IAccountCommandsService
{
    Task<ApplicationResult<AccountOutput?>> CreateAccountAsync(CreateAccountCommand command);

    Task<ApplicationResult<AccountOutput?>> SeedAccountDefaultsAsync(SeedAccountDefaultsCommand command);

    Task<ApplicationResult<AccountOutput?>> AddFinancialMovementAsync(AddFinancialMovementCommand command);

    Task<ApplicationResult<FinancialMovementOutput?>> EditFinancialMovementAsync(EditFinancialMovementCommand command);

    Task<ApplicationResult<FinancialMovementOutput?>> DeleteFinancialMovementAsync(DeleteFinancialMovementCommand command);

    Task<ApplicationResult<AccountOutput?>> InviteMemberByEmailAsync(InviteMemberByEmailCommand command);

    Task<ApplicationResult<AccountOutput?>> InviteMemberByUserAsync(InviteMemberByUserCommand command);

    Task<ApplicationResult<AccountOutput?>> AnswerInviteAsync(AnswerInviteCommand command);

    Task<ApplicationResult<AccountOutput?>> CreateCustomCategoryAsync(CreateCustomCategoryCommand command);

    Task<ApplicationResult<AccountOutput?>> CreateCustomSubCategoryAsync(CreateCustomSubCategoryCommand command);

    Task<ApplicationResult<AccountOutput?>> CreateCustomPaymentMethodAsync(CreateCustomPaymentMethodCommand command);

    Task<ApplicationResult<AccountOutput?>> UpdateAccountDetailsAsync(UpdateAccountDetailsCommand command);

    Task<ApplicationResult<AccountOutput?>> GetByIdAsync(AccountId id, CancellationToken cancellationToken);
}
