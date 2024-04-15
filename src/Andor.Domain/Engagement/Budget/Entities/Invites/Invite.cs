using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Entities.Accounts;
using Andor.Domain.Engagement.Budget.Entities.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Entities.Invites.DomainEvents;
using Andor.Domain.Engagement.Budget.Entities.Invites.ValueObjects;
using Andor.Domain.SeedWork;
using System.Net.Mail;

namespace Andor.Domain.Engagement.Budget.Entities.Invites;

public class Invite : AggregateRoot<InviteId>
{
    public MailAddress Email { get; private set; }
    public InviteStatus Status { get; private set; } = InviteStatus.Undefined;
    public AccountId AccountId { get; private set; }
    public Account? Account { get; private set; }
    private DomainResult SetValues(InviteId id,
        MailAddress email)
    {

        if (Notifications.Count > 1)
        {
            return Validate();
        }

        Id = id;
        Email = email;

        var result = Validate();

        return result;
    }

    public static (DomainResult, Invite?) New(
        MailAddress email)
    {
        var entity = new Invite();

        var result = entity.SetValues(InviteId.New(), email);

        if (result.IsFailure)
        {
            return (result, null);
        }

        entity.RaiseDomainEvent(InviteCreatedDomainEvent.FromAggregator(entity));

        return (result, entity);
    }
}
