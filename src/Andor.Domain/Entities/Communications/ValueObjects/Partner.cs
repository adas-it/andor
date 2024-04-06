using Andor.Domain.Common;

namespace Andor.Domain.Entities.Communications.ValueObjects;

public record Partner : Enumeration<int>
{
    private Partner(int id, string name) : base(id, name)
    {
    }

    public static readonly Partner Undefined = new(0, nameof(Undefined));
    public static readonly Partner InHouse = new(1, nameof(InHouse));
    public static readonly Partner SendGrid = new(2, nameof(SendGrid));
}
