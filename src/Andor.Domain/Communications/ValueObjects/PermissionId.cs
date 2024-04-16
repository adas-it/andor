using Andor.Domain.Validation;

namespace Andor.Domain.Communications.ValueObjects;

public record struct PermissionId(Guid Value)
{
    public static PermissionId New() => new(Guid.NewGuid());

    public static PermissionId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }

        return new PermissionId(guid);
    }

    public static PermissionId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator PermissionId(Guid value) => new(value);

    public static implicit operator Guid(PermissionId id) => id.Value;
}
