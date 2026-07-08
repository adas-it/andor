namespace Andor.Foundation.Contracts.Results;

public record ApplicationErrorCode
{
    protected ApplicationErrorCode(int value) { Value = value; }

    public int Value { get; init; }

    public static ApplicationErrorCode New(int value) => new(value);

    public override string ToString() => Value.ToString();

    public int CompareTo(ApplicationErrorCode? other) => Value.CompareTo(other?.Value);

    public static implicit operator ApplicationErrorCode(int value) => new(value);

    public static implicit operator int(ApplicationErrorCode accountId) => accountId.Value;
}
