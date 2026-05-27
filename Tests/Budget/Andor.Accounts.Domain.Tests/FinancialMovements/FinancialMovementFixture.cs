using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.FinancialMovements;
using Andor.Accounts.Domain.FinancialMovements.ValueObjects;
using Andor.Accounts.Domain.MovementStatuses;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Accounts.Domain.PaymentMethods;
using Andor.Accounts.Domain.SubCategories;
using System.Reflection;

namespace Andor.Accounts.Domain.Tests.FinancialMovements;

internal static class FinancialMovementFixture
{
    /// <summary>
    /// Creates a FinancialMovement for testing purposes using reflection to set private properties.
    /// </summary>
    public static FinancialMovement CreateFinancialMovement(
        DateTime? date = null,
        string? description = null,
        SubCategory? subCategory = null,
        PaymentMethod? paymentMethod = null,
        Account? account = null,
        MovementType? type = null,
        MovementStatus? status = null,
        decimal value = 100.00m)
    {
        // Create instance using private constructor
        var movement = (FinancialMovement)Activator.CreateInstance(
            typeof(FinancialMovement),
            nonPublic: true)!;

        // Set properties using reflection
        SetPrivateProperty(movement, nameof(FinancialMovement.Date), date ?? DateTime.UtcNow);
        SetPrivateProperty(movement, nameof(FinancialMovement.Description), description ?? "Test movement");
        SetPrivateProperty(movement, nameof(FinancialMovement.Value), value);

        if (subCategory != null)
        {
            SetPrivateProperty(movement, nameof(FinancialMovement.SubCategoryId), subCategory.Id);
            SetPrivateProperty(movement, nameof(FinancialMovement.SubCategory), subCategory);
        }

        if (paymentMethod != null)
        {
            SetPrivateProperty(movement, nameof(FinancialMovement.PaymentMethodId), paymentMethod.Id);
            SetPrivateProperty(movement, nameof(FinancialMovement.PaymentMethod), paymentMethod);
        }

        if (account != null)
        {
            SetPrivateProperty(movement, nameof(FinancialMovement.AccountId), account.Id);
            SetPrivateProperty(movement, nameof(FinancialMovement.Account), account);
        }

        SetPrivateProperty(movement, nameof(FinancialMovement.Type), type ?? MovementType.MoneySpending);
        SetPrivateProperty(movement, nameof(FinancialMovement.Status), status ?? MovementStatus.Expected);

        return movement;
    }

    private static void SetPrivateProperty(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName, 
            BindingFlags.Public | BindingFlags.Instance);

        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
        else
        {
            // If property doesn't have public setter, use backing field
            var field = obj.GetType().GetField($"<{propertyName}>k__BackingField", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            field?.SetValue(obj, value);
        }
    }
}
