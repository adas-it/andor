namespace Family.Budget.IntegrationTest.Configurations;

using System;
using Family.Budget.Domain.Entities.Admin;
using Family.Budget.TestsUtil;

public class ConfigurationTestFixture : BaseFixture
{
    public Configuration GetValidConfiguration()
    {
        return Configuration.New(GetStringRigthSize(5, 100),
            GetStringRigthSize(5, 300),
            GetStringRigthSize(5, 1000),
            DateTimeOffset.UtcNow.AddDays(1),
            DateTimeOffset.UtcNow.AddDays(15),
            Guid.NewGuid().ToString()
        );
    }

    [CollectionDefinition(nameof(ConfigurationTestFixture))]
    public class ConfigurationTestFixtureCollection : ICollectionFixture<ConfigurationTestFixture>
    {
    }
}