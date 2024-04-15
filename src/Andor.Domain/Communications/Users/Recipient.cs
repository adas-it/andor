using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Communications;
using Andor.Domain.Communications.Users.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;
using System.Net.Mail;

namespace Andor.Domain.Communications.Users;

public class Recipient : Entity<RecipientId>
{
    public string Name { get; private set; } = "";
    public MailAddress PreferredEmail { get; private set; }
    public bool Active { get; private set; }
    private ICollection<Permission> PrivatePermission { get; set; }
    public IReadOnlyCollection<Permission> Permissions => [.. PrivatePermission];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Recipient()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    private DomainResult SetValues(
        RecipientId id,
        string name,
        MailAddress email,
        bool active,
        ICollection<Permission> permission)
    {
        AddNotification(name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(name.BetweenLength(2, 50));

        if (Notifications.Count > 1)
        {
            return base.Validate();
        }

        Id = id;
        Name = name;
        PreferredEmail = email;
        Active = active;
        PrivatePermission = permission;

        var result = base.Validate();

        return result;
    }

    public static (DomainResult, Recipient?) New(
    string name,
    string email,
    bool active,
    List<Permission> permission)
    {
        var entity = new Recipient();

        var result = entity.SetValues(RecipientId.New(),
        name,
        new MailAddress(email),
        active, permission);

        if (result.IsFailure)
        {
            return (result, null);
        }

        return (result, entity);
    }
}