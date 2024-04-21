using Andor.Domain.Validation;

namespace Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;

public record struct InviteId
{
    private InviteId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        Value = value;
    }
    public Guid Value { get; }
    public static InviteId New() => new(Guid.NewGuid());

    public static InviteId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new InviteId(guid);
    }

    public static InviteId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator InviteId(Guid value) => new(value);

    public static implicit operator Guid(InviteId id) => id.Value;
}
