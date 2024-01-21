namespace Family.Budget.Application.Dto.Models.Events;
public sealed class Versions
{
    private Versions(string value) { Value = value; }

    public string Value { get; private set; }

    public static Versions v1 { get { return new Versions("v1"); } }
}
