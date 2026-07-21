using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.CashFlows.ValueObjects;
using Andor.Foundation.Domain.SeedWork.CommandRepository;

namespace Andor.Accounts.Domain.CashFlows.Repositories;

public interface ICommandsCashFlowRepository : ICommandRepository<CashFlow, CashFlowId>
{
    Task<CashFlow?> GetByAccountAndPeriodAsync(AccountId accountId, int periodKey, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the most recent existing row strictly before the given period, or null if
    /// this would be the account's first-ever CashFlow row.
    /// </summary>
    Task<CashFlow?> GetLatestBeforeAsync(AccountId accountId, int periodKey, CancellationToken cancellationToken);

    /// <summary>
    /// Returns every existing row strictly after the given period, ordered ascending by period.
    /// </summary>
    Task<IReadOnlyList<CashFlow>> GetAfterAsync(AccountId accountId, int periodKey, CancellationToken cancellationToken);
}
