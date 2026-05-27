using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.TestsUtil;

namespace Andor.Accounts.Domain.Tests.Categories;

internal static class CategoryFixture
{
    public static Category GetTemplateCategory(CategoryId? id = null,
        Name? name = null,
        Description? description = null,
        MovementType? type = null)
    => CreateCategory(id: id, name: name, description: description, type: type);

    public static Category GetCustomCategoryWithOwner(
        CategoryId? id = null,
        Name? name = null,
        Description? description = null,
        MovementType? type = null,
        AccountId? owner = null)
    => CreateCategory(id: id, name: name, description: description, type: type, owner: owner);

    private static Category CreateCategory(
        CategoryId? id = null,
        Name? name = null,
        Description? description = null,
        MovementType? type = null,
        AccountId? owner = null)
    {
        name = name ?? GeneralFixture.GetValidName();
        description = description ?? GeneralFixture.GetValidDescription();
        type = type ?? MovementType.MoneySpending;

        var (_, result) = Category.New(
            name: name,
            description: description,
            type: type,
            owner: owner);

        return result!;
    }
}
