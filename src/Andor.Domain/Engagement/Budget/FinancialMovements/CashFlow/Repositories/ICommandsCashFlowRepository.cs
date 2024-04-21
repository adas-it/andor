using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow;
using Andor.Domain.Engagement.Budget.FinancialMovements.CashFlow.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;

public interface ICommandsCashFlowRepository : ICommandRepository<CashFlow, CashFlowId>
{
}
