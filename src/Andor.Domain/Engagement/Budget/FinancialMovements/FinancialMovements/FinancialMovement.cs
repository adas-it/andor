using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Accounts;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementStatuses;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;

public class FinancialMovement : Entity<FinancialMovementId>
{
    public DateTime Date { get; private set; }
    public string? Description { get; private set; }
    public SubCategory SubCategory { get; private set; }
    public MovementType Type { get; private set; }
    public MovementStatus Status { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public AccountId AccountId { get; private set; }
    public Account Account { get; private set; }
    public decimal Value { get; private set; }
    public bool IsDeleted { get; private set; }

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
        AccountId accountId,
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

        var result = Validate();

        return result;
    }

    public static (DomainResult, FinancialMovement) New(FinancialMovementId id,
        DateTime date,
        string? description,
        SubCategory subCategory,
        MovementType type,
        MovementStatus status,
        PaymentMethod paymentMethod,
        AccountId accountId,
        decimal value,
        bool isDeleted)
    {
        var entity = new FinancialMovement();

        var result = entity.SetValues(id, date, description, subCategory, type, status, paymentMethod, accountId, value, isDeleted);

        return (result, entity);
    }
}
