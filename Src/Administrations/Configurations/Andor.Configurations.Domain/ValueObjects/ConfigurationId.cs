using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Configurations.Domain.ValueObjects;

public readonly record struct ConfigurationId : IId<ConfigurationId>
{
    public static ConfigurationId Empty => new ConfigurationId() { Value = Guid.Empty };

    public Guid Value { get; init; }

    private ConfigurationId(Guid value)
    {
        Value = value;
    }

    public static ConfigurationId New() => new ConfigurationId(Guid.NewGuid());

    public static ConfigurationId Load(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException(DefaultsErrorsMessages.InvalidGuid, nameof(value));
        }
        return new ConfigurationId(guid);
    }

    public static ConfigurationId Load(Guid value) => new(value);

    public readonly override string ToString() => Value.ToString();

    public static implicit operator ConfigurationId(Guid value) => new(value);

    public static implicit operator Guid(ConfigurationId id) => id.Value;
}
