namespace Family.Budget.UnitTest.UnitTests.Domain.CashFlow;
using Family.Budget.TestsUtil;
using Xunit;

public class CashFlowTestFixture : CashFlowBaseFixture
{

    [CollectionDefinition(nameof(CashFlowTestFixture))]
    public class CashFlowTestFixtureCollection : ICollectionFixture<CashFlowTestFixture>
    {
    }
}