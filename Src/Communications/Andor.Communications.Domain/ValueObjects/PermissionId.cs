using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain.ValueObjects;

public readonly record struct PermissionId : IId<PermissionId>
{
    public static PermissionId Empty => new PermissionId() { Value = Guid.Empty };

    public Guid Value { get; init; }

    private PermissionId(Guid value)
    {
        Value = value;
    }

    public static PermissionId New() => new PermissionId(Guid.NewGuid());

    public static PermissionId Load(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new PermissionId(guid);
    }

    public static PermissionId Load(Guid value) => new(value);

    public readonly override string ToString() => Value.ToString();

    public static implicit operator PermissionId(Guid value) => new(value);

    public static implicit operator Guid(PermissionId id) => id.Value;
}
