using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.MovementTypes;
using Andor.Accounts.Domain.PaymentMethods.Errors;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.PaymentMethods;

public class PaymentMethod : Entity<PaymentMethodId>, ISoftDeletableEntity
{
    public AccountId? Owner { get; private set; }
    public bool IsTemplate => Owner == null;

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public MovementType Type { get; private set; }
    public bool IsDeleted { get; private set; }

    protected PaymentMethod()
    {
        Name = string.Empty;
        Description = string.Empty;
        Type = MovementType.Undefined;
    }

    private PaymentMethod(PaymentMethodId id, Name name, Description description, MovementType type, AccountId? owner)
    {
        Id = id;
        Name = name;
        Description = description;
        Type = type;
        Owner = owner;
    }

    public static (DomainResult, PaymentMethod?) New(
        PaymentMethodId id,
        Name name,
        Description description,
        MovementType type,
        AccountId? owner)
    {
        var entity = new PaymentMethod(id, name, description, type, owner);
        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

    public static (DomainResult, PaymentMethod?) New(
        Name name,
        Description description,
        MovementType type,
        AccountId? owner)
        => New(PaymentMethodId.New(), name, description, type, owner);

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
                PaymentMethodErrorMessages.MovementTypeCannotBeUndefined, 
                PaymentMethodErrorCode.MovementTypeCannotBeUndefined);
        }

        return base.Validate();
    }
}
