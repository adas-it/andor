using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Accounts.Domain.PaymentMethods;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Accounts.Domain.SubCategories.ValueObjects;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.SubCategories;

public class SubCategory : Entity<SubCategoryId>, ISoftDeletableEntity
{
    public AccountId? Owner { get; private set; }
    public bool IsTemplate => Owner == null;

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public Category Category { get; private set; }
    public MovementType Type => Category.Type;

    public PaymentMethodId? DefaultPaymentMethodId { get; private set; }
    public PaymentMethod? DefaultPaymentMethod { get; private set; }
    public bool IsDeleted { get; private set; }

    protected SubCategory()
    {
    }

    private SubCategory(SubCategoryId id, Name name, Description description, Category category, AccountId? owner)
    {
        Id = id;
        Name = name;
        Description = description;
        Category = category;
        CategoryId = category?.Id ?? default;
        Owner = owner;
    }

    public static (DomainResult, SubCategory?) New(
        SubCategoryId id,
        Name name,
        Description description,
        Category category,
        AccountId? owner)
    {
        var entity = new SubCategory(id, name, description, category, owner);
        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

    public static (DomainResult, SubCategory?) New(
        Name name,
        Description description,
        Category category,
        AccountId? owner)
        => New(SubCategoryId.New(), name, description, category, owner);

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

    public DomainResult SetDefaultPaymentMethod(PaymentMethod? paymentMethod)
    {
        if (paymentMethod != null && paymentMethod.Type != Type)
        {
            return DomainResult.Failure(errors: new List<Notification> { new Notification(AccountErrorMessages.PaymentMethodShouldBeSameTypeAsCategory,
                AccountErrorCode.PaymentMethodShouldBeSameTypeAsCategory) });
        }

        DefaultPaymentMethod = paymentMethod;
        DefaultPaymentMethodId = paymentMethod?.Id;

        return DomainResult.Success();
    }

    protected override DomainResult Validate()
    {
        AddNotification(Name.NotNull());
        AddNotification(Category.NotNull());

        return base.Validate();
    }
}
