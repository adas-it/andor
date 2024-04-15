using Andor.Domain.Validation;

namespace Andor.Domain.Entities.Admin.Configurations.ValueObjects;

public record struct ConfigurationId(Guid Value)
{
    public static ConfigurationId New() => new(Guid.NewGuid());

    public static ConfigurationId Load(string value)
    {
        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new ConfigurationId(guid);
    }

    public static ConfigurationId Load(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();

    public static implicit operator ConfigurationId(Guid value) => new(value);

    public static implicit operator Guid(ConfigurationId id) => id.Value;
}
