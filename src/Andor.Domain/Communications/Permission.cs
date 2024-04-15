using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Communications.Users;
using Andor.Domain.Communications.Users.ValueObjects;
using Andor.Domain.Communications.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Communications;

public class Permission : Entity<PermissionId>
{
    public RecipientId RecipientId { get; private set; }
    public Recipient Recipient { get; private set; }
    public ValueObjects.Type Type { get; private set; }
    public bool Consented { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Permission()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    private DomainResult SetValues(
        PermissionId permissionId,
        Recipient recipient,
        ValueObjects.Type type,
        bool consented)
    {
        AddNotification(recipient.NotNull());

        if (Notifications.Count > 1)
        {
            return base.Validate();
        }

        Id = permissionId;
        Recipient = recipient;
        Type = type;
        Consented = consented;
        RecipientId = recipient.Id;

        var result = base.Validate();

        return result;
    }

    public static (DomainResult, Permission?) New(Recipient recipient,
        ValueObjects.Type type,
        bool consented)
    {
        var entity = new Permission();

        var response = entity.SetValues(PermissionId.New(), recipient, type, consented);

        return (response, entity);
    }
}
