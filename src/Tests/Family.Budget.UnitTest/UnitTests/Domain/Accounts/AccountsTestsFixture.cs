namespace Family.Budget.UnitTest.UnitTests.Domain.CashFlow;
using Family.Budget.TestsUtil;
using Xunit;

public class AccountsTestsFixture : AccountsBaseFixture
{

    [CollectionDefinition(nameof(AccountsTestsFixture))]
    public class AccountsTestsFixtureCollection : ICollectionFixture<AccountsTestsFixture>
    {
    }
}