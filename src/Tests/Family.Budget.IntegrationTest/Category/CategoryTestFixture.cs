namespace Family.Budget.IntegrationTest.Category;
using Family.Budget.TestsUtil;
public class CategoryTestFixture : CategoryBaseFixture
{
    [CollectionDefinition(nameof(CategoryTestFixture))]
    public class ConfigurationTestFixtureCollection : ICollectionFixture<CategoryTestFixture>
    {
    }
}
