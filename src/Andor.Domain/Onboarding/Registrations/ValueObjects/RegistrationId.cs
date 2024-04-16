using Andor.Domain.Validation;

namespace Andor.Domain.Onboarding.Registrations.ValueObjects;
public record struct RegistrationId(Guid Value)
{
    public static RegistrationId New() => new(Guid.NewGuid());

    public static RegistrationId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new RegistrationId(guid);
    }

    public static RegistrationId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator RegistrationId(Guid value) => new(value);

    public static implicit operator Guid(RegistrationId id) => id.Value;
}
