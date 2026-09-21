using Andor.Communications.Domain.Users.ValueObjects;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain.Messages;

/// <summary>
/// A push notification recorded for in-app delivery: PushPartner writes one of these instead of
/// calling an external push provider (not wired up yet), and the recipient's app lists them via
/// GET /v1/communications/messages.
/// </summary>
public class Message : Entity<MessageId>
{
    public RecipientId RecipientId { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public DateTimeOffset SentAt { get; private set; }

    private Message()
    {
        Title = string.Empty;
        Body = string.Empty;
    }

    private Message(MessageId id, RecipientId recipientId, string title, string body, DateTimeOffset sentAt)
    {
        Id = id;
        RecipientId = recipientId;
        Title = title;
        Body = body;
        SentAt = sentAt;
    }

    public static (DomainResult, Message?) New(RecipientId recipientId, string title, string body)
    {
        var entity = new Message(MessageId.New(), recipientId, title, body, DateTimeOffset.UtcNow);

        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

    protected override DomainResult Validate()
    {
        AddNotification(Title.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Body.NotNullOrEmptyOrWhiteSpace());

        return base.Validate();
    }
}
