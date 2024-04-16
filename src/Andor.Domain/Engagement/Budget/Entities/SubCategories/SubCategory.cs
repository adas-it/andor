using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Entities.Categories;
using Andor.Domain.Engagement.Budget.Entities.Categories.ValueObjects;
using Andor.Domain.Engagement.Budget.Entities.PaymentMethods;
using Andor.Domain.Engagement.Budget.Entities.PaymentMethods.ValueObjects;
using Andor.Domain.Engagement.Budget.Entities.SubCategories.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.Entities.SubCategories;

public class SubCategory : AggregateRoot<SubCategoryId>
{
    public string Name { get; private set; } = "";
    public string Description { get; private set; } = "";
    public DateTime? StartDate { get; private set; }
    public DateTime? DeactivationDate { get; private set; }
    public CategoryId? CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public PaymentMethodId? DefaultPaymentMethodId { get; private set; }
    public PaymentMethod? DefaultPaymentMethod { get; private set; }
    private DomainResult SetValues(SubCategoryId id,
        string name)
    {
        AddNotification(name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(name.BetweenLength(3, 70));

        if (Notifications.Count > 1)
        {
            return Validate();
        }

        Id = id;
        Name = name;

        var result = Validate();

        return result;
    }

    public static (DomainResult, SubCategory?) New(
        string name)
    {
        var entity = new SubCategory();

        var result = entity.SetValues(SubCategoryId.New(), name);

        if (result.IsFailure)
        {
            return (result, null);
        }

        return (result, entity);
    }
}
