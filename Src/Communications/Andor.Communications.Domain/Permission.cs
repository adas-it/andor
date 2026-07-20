using Andor.Communications.Domain.Users;
using Andor.Communications.Domain.Users.ValueObjects;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain;

public class Permission : Entity<PermissionId>
{
    public RecipientId RecipientId { get; private set; }
    public Recipient? Recipient { get; private set; }
    public ValueObjects.Type Type { get; private set; }
    public bool Consented { get; private set; }

    private Permission()
    {
        Type = ValueObjects.Type.Undefined;
    }

    private Permission(
        PermissionId permissionId,
        Recipient recipient,
        ValueObjects.Type type,
        bool consented)
    {
        Id = permissionId;
        Recipient = recipient;
        Type = type;
        Consented = consented;
        RecipientId = recipient.Id;
    }

    public static (DomainResult, Permission?) New(Recipient recipient,
        ValueObjects.Type type,
        bool consented)
    {
        var entity = new Permission(PermissionId.New(), recipient, type, consented);

        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

    protected override DomainResult Validate()
    {
        AddNotification(Recipient.NotNull());

        return base.Validate();
    }
}
