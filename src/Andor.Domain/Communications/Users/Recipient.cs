using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Entities.Communications.Users.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;
using System.Net.Mail;

namespace Andor.Domain.Communications.Users;

public class Recipient : Entity<RecipientId>
{
    public string Name { get; private set; }
    public MailAddress PreferredEmail { get; private set; }
    public bool Active { get; private set; }
    public ICollection<Permission> Permissions { get; private set; }

    private Recipient()
    {
        Id = RecipientId.New();
        Name = string.Empty;
        PreferredEmail = new MailAddress(string.Empty);
        Active = false;
        Permissions = [];
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
        Permissions = permission;

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