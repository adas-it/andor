using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Entities.Communications.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Entities.Communications;

public class Template : Entity<TemplateId>
{
    public string Value { get; private set; }
    public string ContentLanguage { get; private set; }
    public string Title { get; private set; }
    public Partner Partner { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public RuleId RuleId { get; private set; }
    public Rule Rule { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Template()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    private DomainResult SetValues(
        TemplateId id,
        string value,
        string contentLanguage,
        string title,
        Partner partner,
        DateTime createdAt,
        Rule rule)
    {
        AddNotification(title.NotNullOrEmptyOrWhiteSpace());
        AddNotification(title.BetweenLength(2, 50));

        if (Notifications.Count > 1)
        {
            return base.Validate();
        }

        Id = id;
        Value = value;
        ContentLanguage = contentLanguage;
        Title = title;
        Partner = partner;
        CreatedAt = createdAt;
        Rule = rule;
        RuleId = rule.Id;

        var result = base.Validate();

        return result;
    }

    public static (DomainResult, Template?) New(
        string value,
        string contentLanguage,
        string title,
        Partner partner,
        Rule rule)
    {
        var entity = new Template();

        var response = entity.SetValues(
            TemplateId.New(),
            value,
            contentLanguage,
            title,
            partner,
            DateTime.UtcNow,
            rule);

        return (response, entity);
    }
}
