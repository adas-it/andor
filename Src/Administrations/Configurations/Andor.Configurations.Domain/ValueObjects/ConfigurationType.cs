using Andor.Foundation.Domain;

namespace Andor.Configurations.Domain.ValueObjects;

public record ConfigurationType : Enumeration<int>
{
    private ConfigurationType(int id, string name) : base(id, name)
    {
    }

    public static readonly ConfigurationType Generic = new(0, nameof(Generic));
    public static readonly ConfigurationType Corporate = new(1, nameof(Corporate));
    public static readonly ConfigurationType Restricted = new(2, nameof(Restricted));
}
