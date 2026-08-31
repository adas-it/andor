using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain;

public class Template : Entity<TemplateId>
{
    public string Value { get; private set; }
    public string ContentLanguage { get; private set; }
    public string Title { get; private set; }
    public string Subject { get; private set; }
    public Partner Partner { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public RuleId RuleId { get; private set; }
    public Rule? Rule { get; private set; }
    public bool IsDefault { get; private set; }

    protected Template()
    {
        Id = TemplateId.New();
        Value = string.Empty;
        ContentLanguage = string.Empty;
        Title = string.Empty;
        Subject = string.Empty;
        Partner = Partner.Undefined;
    }

    private Template(
        TemplateId id,
        string value,
        string contentLanguage,
        string title,
        string subject,
        Partner partner,
        DateTime createdAt,
        Rule rule,
        bool isDefault)
    {
        Id = id;
        Value = value;
        ContentLanguage = contentLanguage;
        Title = title;
        Subject = subject;
        Partner = partner;
        CreatedAt = createdAt;
        Rule = rule;
        RuleId = rule.Id;
        IsDefault = isDefault;
    }

    public static (DomainResult, Template?) New(
        string value,
        string contentLanguage,
        string title,
        string subject,
        Partner partner,
        Rule rule,
        bool isDefault)
    {
        var entity = new Template(
            TemplateId.New(),
            value,
            contentLanguage,
            title,
            subject,
            partner,
            DateTime.UtcNow,
            rule,
            isDefault);

        var result = entity.Validate();

        return result.IsFailure
            ? (result, null)
            : (result, entity);
    }

    protected override DomainResult Validate()
    {
        AddNotification(Title.NotNullOrEmptyOrWhiteSpace());
        AddNotification(Title.BetweenLength(2, 50));

        return base.Validate();
    }
}
