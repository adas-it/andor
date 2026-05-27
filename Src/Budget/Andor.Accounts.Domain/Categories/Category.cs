using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories.Errors;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Accounts.Domain.SubCategories;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Categories;

public class Category : Entity<CategoryId>, ISoftDeletableEntity
{
    public AccountId? Owner { get; private set; }
    public bool IsTemplate => Owner == null;

    public Name Name { get; private set; }
    public Description? Description { get; private set; }
    public MovementType Type { get; init; }
    public ICollection<SubCategory> SubCategories { get; private set; }
    public bool IsDeleted { get; private set; }

    protected Category()
    {
        SubCategories = [];
        Type = MovementType.Undefined;
    }

    private Category(CategoryId id, Name name, Description? description, MovementType type, AccountId? owner)
    {
        Id = id;
        Name = name;
        Description = description;
        Type = type;
        Owner = owner;
        SubCategories = [];
        Type = type;
    }

    public static (DomainResult, Category?) New(
        CategoryId id,
        Name name,
        Description description,
        MovementType type,
        AccountId? owner)
    {
        var entity = new Category(id, name, description, type, owner);
        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

    public static (DomainResult, Category?) New(
        Name name,
        Description description,
        MovementType type,
        AccountId? owner)
        => New(CategoryId.New(), name, description, type, owner);

    public DomainResult SoftDelete()
    {
        IsDeleted = true;
        return DomainResult.Success();
    }

    public DomainResult Restore()
    {
        IsDeleted = false;
        return DomainResult.Success();
    }

    protected override DomainResult Validate()
    {
        AddNotification(Name.NotNull());

        if (Type == MovementType.Undefined)
        {
            AddNotification(nameof(Type),
                CategoryErrorMessages.MovementTypeCannotBeUndefined,
                CategoryErrorCode.MovementTypeCannotBeUndefined);
        }

        return base.Validate();
    }
}
