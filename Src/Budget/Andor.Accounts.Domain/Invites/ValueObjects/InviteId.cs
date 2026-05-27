using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Invites.ValueObjects;

public record struct InviteId : IId<InviteId>
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

    public static InviteId Load(Guid value) => new(value);

    public static InviteId Load(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        return new InviteId(guid);
    }

    public static implicit operator Guid(InviteId id) => id.Value;
    public static implicit operator InviteId(Guid value) => new(value);
}
