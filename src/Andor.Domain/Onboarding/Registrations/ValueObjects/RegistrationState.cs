using Andor.Domain.Common;

namespace Andor.Domain.Onboarding.Registrations.ValueObjects;

public record RegistrationState : Enumeration<int>
{
    private RegistrationState(int id, string name) : base(id, name)
    {
    }

    public static readonly RegistrationState Undefined = new(0, nameof(Undefined));
    public static readonly RegistrationState GeneratedCode = new(1, nameof(GeneratedCode));
    public static readonly RegistrationState Notified = new(2, nameof(Notified));
    public static readonly RegistrationState Completed = new(3, nameof(Completed));
}
