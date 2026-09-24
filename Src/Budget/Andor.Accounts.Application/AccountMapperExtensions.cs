using Andor.Accounts.Contracts.Accounts.Responses;
using Andor.Accounts.Contracts.Categories.Response;
using Andor.Accounts.Contracts.FinancialMovements.Response;
using Andor.Accounts.Contracts.FinancialMovementStatuses;
using Andor.Accounts.Contracts.Invites.Responses;
using Andor.Accounts.Contracts.MovementTypes;
using Andor.Accounts.Contracts.PaymentMethods.Responses;
using Andor.Accounts.Contracts.SubCategories.Responses;
using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Categories;
using Andor.Accounts.Domain.FinancialMovements;
using Andor.Accounts.Domain.Invites;
using Andor.Accounts.Domain.PaymentMethods;
using Andor.Accounts.Domain.SubCategories;

namespace Andor.Accounts.Application;

internal static class AccountMapperExtensions
{
    public static AccountOutput? ToAccountOutput(this Account? entity)
    {
        if (entity == null)
            return null;

        return new AccountOutput()
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description,
            Deleted = entity.IsDeleted,
            Currency = new CurrencyOutput(entity.Currency.Id.ToString(), entity.Currency.Name, entity.Currency.Symbol),
            Participants = entity.Members
                .Select(m => new ParticipantOutput()
                {
                    Id = m.UserId.ToString(),
                    PermissionType = new PermissionTypeOutput(m.PermissionType.Key, m.PermissionType.Name)

                })
                .ToList()
        };
    }

    public static SubCategoryOutput? ToSubCategoryOutput(this SubCategory? entity, int order)
    {
        if (entity == null)
            return null;

        return new SubCategoryOutput()
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description,
            Category = entity.Category.ToCategoryOutput(null),
            DefaultPaymentMethod = entity.DefaultPaymentMethod?.ToPaymentMethodOutput(null),
            Order = order,
            IsTemplate = entity.IsTemplate,
        };
    }

    public static CategoryOutput? ToCategoryOutput(this Category? entity, int? order)
    {
        if (entity == null)
            return null;

        return new CategoryOutput()
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description,
            Type = new CategoryTypeOutput(entity.Type.Key, entity.Type.Name),
            Order = order,
            IsTemplate = entity.IsTemplate,
        };
    }

    public static PaymentMethodOutput? ToPaymentMethodOutput(this PaymentMethod? entity, int? order)
    {
        if (entity == null)
            return null;

        return new PaymentMethodOutput()
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description,
            Order = order,
            IsTemplate = entity.IsTemplate,
        };
    }

    public static InviteOutput? ToInviteOutput(this Invite? entity)
    {
        if (entity == null)
            return null;

        return new InviteOutput()
        {
            Id = entity.Id.ToString(),
            AccountId = entity.AccountId.ToString(),
            Email = entity.Email?.Value,
            UserId = entity.UserId?.ToString(),
            PermissionKey = entity.Permission.Key,
            PermissionName = entity.Permission.Name,
            IsActive = entity.IsActive,
            IsAccepted = entity.IsAccepted,
        };
    }

    public static FinancialMovementOutput? ToFinancialMovementOutput(this FinancialMovement? entity)
    {
        if (entity == null)
            return null;

        return new FinancialMovementOutput()
        {
            Id = entity.Id.Value,
            Date = entity.Date,
            Description = entity.Description,
            Value = entity.Value,
            SubCategory = entity.SubCategory.ToSubCategoryOutput(0)!,
            Type = new MovementTypeOutput(entity.Type.Key, entity.Type.Name),
            Status = new FinancialMovementStatusOutput(entity.Status.Key, entity.Status.Name),
            PaymentMethod = entity.PaymentMethod.ToPaymentMethodOutput(null)!,
        };
    }
}
