using Andor.Accounts.Application.Commands;
using Andor.Accounts.Contracts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Application.Interfaces;

public interface IAccountCommandsService
{
    Task<ApplicationResult<AccountOutput?>> CreateAccountAsync(CreateAccountCommand command);

    Task<ApplicationResult<AccountOutput?>> SeedAccountDefaultsAsync(SeedAccountDefaultsCommand command);

    Task<ApplicationResult<AccountOutput?>> AddFinancialMovementAsync(AddFinancialMovementCommand command);

    Task<ApplicationResult<AccountOutput?>> GetByIdAsync(AccountId id, CancellationToken cancellationToken);
}
