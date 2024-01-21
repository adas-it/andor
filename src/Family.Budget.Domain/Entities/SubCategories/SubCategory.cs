namespace Family.Budget.Domain.Entities.SubCategories;


using Family.Budget.Domain.Entities.Categories;
using Family.Budget.Domain.Entities.PaymentMethods;
using Family.Budget.Domain.Entities.SubCategories.DomainEvents;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.Validation;
using System;

public class SubCategory : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset? StartDate { get; private set; }
    public DateTimeOffset? DeactivationDate { get; private set; }
    public Category Category { get; private set; }
    public PaymentMethod? DefaultPaymentMethod { get; private set; }

    private SubCategory()
    {
    }

    private SubCategory(Guid id, string name, string description, 
        DateTimeOffset? startDate, DateTimeOffset? deactivationDate,
        Category category, PaymentMethod? defaultPaymentMethod)
    {
        Id = id;
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = deactivationDate;
        Category = category;
        DefaultPaymentMethod = defaultPaymentMethod;

        Validate();
    }

    public static SubCategory New(string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate,
        Category category, PaymentMethod defaultPaymentMethod)
    {
        var entity = new SubCategory(Guid.NewGuid(),
            name,
            description,
            startDate,
            deactivationDate,
            category,
            defaultPaymentMethod);

        entity.RaiseDomainEvent(new SubCategoryCreatedDomainEvent(entity));

        return entity;
    }

    protected override void Validate()
    {
        AddNotification(Name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Name.BetweenLength(3, 100));

        AddNotification(StartDate.NotDefaultDateTime());

        AddNotification(DeactivationDate.NotDefaultDateTime());

        if (DeactivationDate.GetValueOrDefault() != (default) && StartDate.GetValueOrDefault() != (default)
            && DeactivationDate!.Value < StartDate!)
        {
            //var message = DefaultsErrorsMessages.Date0CannotBeBeforeDate1.GetMessage(nameof(DeactivationDate), nameof(StartDate));
         
            //AddNotification(new (nameof(DeactivationDate), message, ErrorsCodes.CategoryDateConflit));
        }

        AddNotification(Category.NotNull());

        base.Validate();
    }

    public void SetSubCategory(string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate,
        Category category)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        DeactivationDate = deactivationDate;
        Category = category;

        Validate();

        RaiseDomainEvent(new SubCategoryChangedDomainEvent(this));
    }
}
