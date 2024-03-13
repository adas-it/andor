namespace Family.Budget.Domain.Entities.Admin;

using Family.Budget.Domain.Common.ValuesObjects;

public sealed record ConfigurationsErrorsCodes : DomainErrorCode
{
    private ConfigurationsErrorsCodes(int original) : base(original)
    {
    }

    public static readonly ConfigurationsErrorsCodes Validation = new(3_000);
}
