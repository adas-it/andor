namespace Family.Budget.Domain.Entities.CashFlow.Repository;
using System;
using System.Threading.Tasks;

public interface ICashFlowRepository
{
    Task Insert(CashFlow entity, CancellationToken cancellationToken);
    Task Update(CashFlow entity, CancellationToken cancellationToken);
    Task Delete(CashFlow entity, CancellationToken cancellationToken);
    Task Delete(Guid id, CancellationToken cancellationToken);
    Task<CashFlow?> GetByAccountIdAsync(Guid accountId, int year, int month, CancellationToken cancellationToken);
    Task<CashFlow?> GetPreviousCashFlowByAccountIdAsync(Guid accountId, int year, int month, CancellationToken cancellationToken);
    Task<CashFlow?> GetNextCashFlowByAccountIdAsync(Guid accountId, int year, int month, CancellationToken cancellationToken);
    Task<CashFlow?> GetCurrentOrPreviousCashFlowByAccountIdAsync(Guid accountId, int year, int month, CancellationToken cancellationToken);
}
