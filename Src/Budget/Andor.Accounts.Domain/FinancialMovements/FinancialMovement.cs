using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements.Errors;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Accounts.Domain.MovementStatuses;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Accounts.Domain.PaymentMethods;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Accounts.Domain.SubCategories;
using Andor.Accounts.Domain.SubCategories.ValueObjects;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.FinancialMovements;

/// <summary>
/// Represents a financial movement entity that tracks income or expenses within an account.
/// </summary>
public class FinancialMovement : Entity<FinancialMovementId>, ISoftDeletableEntity
{
    /// <summary>
    /// Gets the date when the financial movement occurred.
    /// </summary>
    public DateTime Date { get; private set; }

    /// <summary>
    /// Gets the optional description of the financial movement.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the subcategory identifier associated with this movement.
    /// </summary>
    public SubCategoryId SubCategoryId { get; private set; }

    /// <summary>
    /// Gets the subcategory associated with this movement.
    /// </summary>
    public SubCategory SubCategory { get; private set; }

    /// <summary>
    /// Gets the type of movement (Income, Expense, Transfer, etc.).
    /// </summary>
    public MovementType Type => SubCategory.Type;

    /// <summary>
    /// Gets the current status of the movement (Pending, Completed, Cancelled, etc.).
    /// </summary>
    public MovementStatus Status { get; private set; }

    /// <summary>
    /// Gets the payment method identifier used for this movement.
    /// </summary>
    public PaymentMethodId PaymentMethodId { get; private set; }

    /// <summary>
    /// Gets the payment method used for this movement.
    /// </summary>
    public PaymentMethod PaymentMethod { get; private set; }

    /// <summary>
    /// Gets the account identifier that owns this movement.
    /// </summary>
    public AccountId AccountId { get; private set; }

    /// <summary>
    /// Gets the account that owns this movement.
    /// </summary>
    public Account Account { get; private set; }

    /// <summary>
    /// Gets the monetary value of the movement.
    /// </summary>
    public decimal Value { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the movement is soft deleted.
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Parameterless constructor for ORM use only.
    /// </summary>
    protected FinancialMovement()
    {
        Id = FinancialMovementId.New();
        Description = string.Empty;
        Status = MovementStatus.Undefined;
    }

    /// <summary>
    /// Private constructor for creating a financial movement with all required parameters.
    /// </summary>
    private FinancialMovement(
        FinancialMovementId id,
        DateTime date,
        string? description,
        SubCategory subCategory,
        PaymentMethod paymentMethod,
        Account account,
        decimal value,
        MovementStatus status)
    {
        Id = id;
        Date = date;
        Description = description;
        SubCategory = subCategory;
        SubCategoryId = subCategory.Id;
        PaymentMethod = paymentMethod;
        PaymentMethodId = paymentMethod.Id;
        Account = account;
        AccountId = account.Id;
        Value = value;
        Status = status;
    }

    /// <summary>
    /// Creates a new financial movement with a specified ID.
    /// </summary>
    /// <param name="id">The unique identifier for the financial movement.</param>
    /// <param name="date">The date when the movement occurred.</param>
    /// <param name="description">Optional description of the movement.</param>
    /// <param name="subCategory">The subcategory associated with this movement. The movement type is derived from this subcategory.</param>
    /// <param name="paymentMethod">The payment method used for this movement.</param>
    /// <param name="account">The account that owns this movement.</param>
    /// <param name="value">The monetary value of the movement.</param>
    /// <param name="status">The status of the movement (defaults to Expected).</param>
    /// <returns>A tuple containing the domain result and the created financial movement (null if validation fails).</returns>
    public static (DomainResult, FinancialMovement?) New(
        FinancialMovementId id,
        DateTime date,
        string? description,
        SubCategory subCategory,
        PaymentMethod paymentMethod,
        Account account,
        decimal value,
        MovementStatus? status = null)
    {
        var entity = new FinancialMovement(
            id,
            date,
            description,
            subCategory,
            paymentMethod,
            account,
            value,
            status ?? MovementStatus.Expected);

        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

    /// <summary>
    /// Creates a new financial movement with an auto-generated ID.
    /// </summary>
    /// <param name="date">The date when the movement occurred.</param>
    /// <param name="description">Optional description of the movement.</param>
    /// <param name="subCategory">The subcategory associated with this movement. The movement type is derived from this subcategory.</param>
    /// <param name="paymentMethod">The payment method used for this movement.</param>
    /// <param name="account">The account that owns this movement.</param>
    /// <param name="value">The monetary value of the movement.</param>
    /// <param name="status">The status of the movement (defaults to Expected).</param>
    /// <returns>A tuple containing the domain result and the created financial movement (null if validation fails).</returns>
    public static (DomainResult, FinancialMovement?) New(
        DateTime date,
        string? description,
        SubCategory subCategory,
        PaymentMethod paymentMethod,
        Account account,
        decimal value,
        MovementStatus? status = null)
        => New(
            FinancialMovementId.New(),
            date,
            description,
            subCategory,
            paymentMethod,
            account,
            value,
            status);

    /// <summary>
    /// Soft deletes the financial movement, marking it as deleted without removing it from the database.
    /// </summary>
    /// <returns>A domain result indicating success.</returns>
    public DomainResult SoftDelete()
    {
        IsDeleted = true;
        return DomainResult.Success();
    }

    /// <summary>
    /// Restores a soft-deleted financial movement.
    /// </summary>
    /// <returns>A domain result indicating success.</returns>
    public DomainResult Restore()
    {
        IsDeleted = false;
        return DomainResult.Success();
    }

    /// <summary>
    /// Validates the financial movement entity.
    /// </summary>
    /// <returns>A domain result indicating validation success or failure.</returns>
    protected override DomainResult Validate()
    {
        AddNotification(SubCategory.NotNull());
        AddNotification(PaymentMethod.NotNull());
        AddNotification(Account.NotNull());

        if (Value <= 0)
        {
            AddNotification(nameof(Value),
                FinancialMovementErrorMessages.ValueMustBeGreaterThanZero,
                FinancialMovementErrorCode.ValueMustBeGreaterThanZero);
        }

        if (Date > DateTime.UtcNow.AddYears(5))
        {
            AddNotification(nameof(Date),
                FinancialMovementErrorMessages.DateCannotBeTooFarInFuture,
                FinancialMovementErrorCode.DateCannotBeTooFarInFuture);
        }

        // Validate that PaymentMethod Type matches the Movement Type (from SubCategory)
        if (PaymentMethod != null && SubCategory != null && PaymentMethod.Type != SubCategory.Type)
        {
            AddNotification(nameof(PaymentMethod),
                FinancialMovementErrorMessages.PaymentMethodTypeMismatch,
                FinancialMovementErrorCode.PaymentMethodTypeMismatch);
        }

        return base.Validate();
    }
}
