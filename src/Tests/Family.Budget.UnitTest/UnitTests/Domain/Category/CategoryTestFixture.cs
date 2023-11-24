namespace Family.Budget.UnitTest.UnitTests.Domain.Category;
using System;
using Xunit;
using Family.Budget.Domain.Entities.Categories;
using Family.Budget.TestsUtil;

public class CategoryTestFixture : CategoryBaseFixture
{

    [CollectionDefinition(nameof(CategoryTestFixture))]
    public class CategoryTestFixtureCollection : ICollectionFixture<CategoryTestFixture>
    {
    }
}