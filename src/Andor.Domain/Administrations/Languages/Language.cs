using Andor.Domain.Administrations.Languages.ValueObjects;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Entities.Currencies.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Administrations.Languages;

public class Language : Entity<LanguageId>
{
    public string Name { get; private set; } = string.Empty;
    public Iso Iso { get; private set; } = string.Empty;
    public string Symbol { get; private set; } = string.Empty;

    private Language()
    {
        Validate();
    }

    public static Language New(string name,
        string iso,
        string symbol)
    {
        var entity = new Language()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Iso = iso,
            Symbol = symbol
        };

        return entity;
    }

    protected override DomainResult Validate()
    {
        AddNotification(Name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Name.BetweenLength(3, 100));

        AddNotification(Iso.NotNull());

        return base.Validate();
    }
}
