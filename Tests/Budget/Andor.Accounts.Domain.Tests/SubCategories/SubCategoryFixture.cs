using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories;
using Andor.Accounts.Domain.SubCategories;
using Andor.Accounts.Domain.Tests.Categories;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;

namespace Andor.Accounts.Domain.Tests.SubCategories;

internal static class SubCategoryFixture
{
    public static SubCategory GetTemplateSubCategory(
        string? name = null,
        Category? category = null)
    {
        Name nameVO = name ?? GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        category ??= CategoryFixture.GetTemplateCategory();

        var (_, result) = SubCategory.New(
            name: nameVO,
            description: description,
            category: category,
            owner: null);

        return result!;
    }

    public static SubCategory GetCustomSubCategoryWithOwner(
        string? name = null,
        Category? category = null,
        AccountId? owner = null)
    {
        Name nameVO = name ?? GeneralFixture.GetValidName();
        var description = GeneralFixture.GetValidDescription();
        category ??= CategoryFixture.GetCustomCategoryWithOwner(owner: owner);

        var (_, result) = SubCategory.New(
            name: nameVO,
            description: description,
            category: category,
            owner: owner);

        return result!;
    }
}
