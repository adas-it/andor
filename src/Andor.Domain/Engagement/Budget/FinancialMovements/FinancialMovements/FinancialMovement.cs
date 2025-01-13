using Andor.Domain.Common;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.DomainEvents;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementStatuses;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;

public class FinancialMovement : AggregateRoot<FinancialMovementId>, ISoftDeletableEntity
{
    public DateTime Date { get; private set; }
    public string? Description { get; private set; }
    public SubCategoryId SubCategoryId { get; private set; }
    public SubCategory SubCategory { get; private set; }
    public MovementType Type { get; private set; }
    public MovementStatus Status { get; private set; }
    public PaymentMethodId PaymentMethodId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public AccountId AccountId { get; private set; }
    public Account Account { get; private set; }
    public decimal Value { get; private set; }
    public bool IsDeleted { get; private set; }
    public bool IsItCreditHandling => CheckIsItCreditHandling();

    private FinancialMovement()
    {
    }

    private DomainResult SetValues(FinancialMovementId id,
        DateTime date,
        string? description,
        SubCategory subCategory,
        MovementType type,
        MovementStatus status,
        PaymentMethod paymentMethod,
        Account account,
        decimal value,
        bool isDeleted)
    {
        AddNotification(description.NotNullOrEmptyOrWhiteSpace());
        AddNotification(description.BetweenLength(3, 300));

        if (Notifications.Count > 1)
        {
            return Validate();
        }

        Id = id;
        Description = description;
        Date = date;
        SubCategoryId = subCategory.Id;
        SubCategory = subCategory;
        Type = type;
        Status = status;
        PaymentMethodId = paymentMethod.Id;
        PaymentMethod = paymentMethod;
        AccountId = account.Id;
        Account = account;
        Value = value;
        IsDeleted = isDeleted;

        var result = Validate();

        return result;
    }

    private bool CheckIsItCreditHandling()
    {
        var CreditPaymentMethodId = new PaymentMethodId[] {
            PaymentMethodId.Load("b72a24e3-7c5f-4790-9a3b-959ddf3c8315"),
            PaymentMethodId.Load("93cef651-35c5-4486-81e4-a61962695b81") };

        return CreditPaymentMethodId.Contains(PaymentMethodId);
    }

    public static (DomainResult, FinancialMovement) New(DateTime date,
        string? description,
        SubCategory subCategory,
        MovementType type,
        MovementStatus status,
        PaymentMethod paymentMethod,
        Account account,
        decimal value)
    {
        var entity = new FinancialMovement();

        var result = entity.SetValues(FinancialMovementId.New(),
            date,
            description,
            subCategory,
            type,
            status,
            paymentMethod,
            account,
            value,
            false);

        entity.RaiseDomainEvent(new FinancialMovementCreatedDomainEvent()
        {
            Current = FinancialMovementDomainEvent.FromAggregator(entity)
        });

        return (result, entity);
    }

    public (DomainResult, FinancialMovement) Update(DateTime date,
        string? description,
        SubCategory subCategory,
        MovementType type,
        MovementStatus status,
        PaymentMethod paymentMethod,
        decimal value)
    {
        var domainEvent = new FinancialMovementChangedDomainEvent()
        {
            Old = FinancialMovementDomainEvent.FromAggregator(this),
        };

        var result = SetValues(Id,
            date,
            description,
            subCategory,
            type,
            status,
            paymentMethod,
            Account,
            value,
            false);

        domainEvent.Current = FinancialMovementDomainEvent.FromAggregator(this);

        this.RaiseDomainEvent(domainEvent);

        return (result, this);
    }

    public (DomainResult, FinancialMovement) Delete()
    {
        var domainEvent = new FinancialMovementDeletedDomainEvent()
        {
            Current = FinancialMovementDomainEvent.FromAggregator(this),
        };

        var result = SetValues(Id,
            Date,
            Description,
            SubCategory,
            Type,
            Status,
            PaymentMethod,
            Account,
            Value,
            true);

        domainEvent.Current = FinancialMovementDomainEvent.FromAggregator(this);

        this.RaiseDomainEvent(domainEvent);

        return (result, this);
    }
}
