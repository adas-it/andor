namespace Family.Budget.TestsUtil;

using Family.Budget.Domain.Entities.CashFlow;
using System;

public class CashFlowBaseFixture : AccountsBaseFixture
{
    public CashFlow GetValidCashFlow(Guid? accountId)
    {
        return CashFlow.New(
            DateTime.UtcNow.Year,
            DateTime.UtcNow.Month,
            accountId ?? Guid.NewGuid(),
            0);
    }
}
