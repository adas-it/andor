using System.Net.Mail;
using Andor.Communications.Domain.Users.ValueObjects;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain.Users;

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

    private Recipient(
        RecipientId id,
        string name,
        MailAddress email,
        bool active,
        ICollection<Permission> permissions)
    {
        Id = id;
        Name = name;
        PreferredEmail = email;
        Active = active;
        Permissions = permissions;
    }

    public static (DomainResult, Recipient?) New(
        string name,
        string email,
        bool active,
        List<Permission> permission)
    {
        var entity = new Recipient(
            RecipientId.New(),
            name,
            new MailAddress(email),
            active,
            permission);

        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

    protected override DomainResult Validate()
    {
        AddNotification(Name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Name.BetweenLength(2, 50));

        return base.Validate();
    }
}