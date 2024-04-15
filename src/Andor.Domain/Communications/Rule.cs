using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Communications.ValueObjects;
using Andor.Domain.SeedWork;
using Andor.Domain.Validation;

namespace Andor.Domain.Communications;

public class Rule : AggregateRoot<RuleId>
{
    public string Name { get; private set; } = "";
    public ValueObjects.Type Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    //private List<Template> PrivateTemplates { get; set; } = [];
    public ICollection<Template> Templates { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Rule()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    private DomainResult SetValues(
        RuleId id,
        string name,
        ValueObjects.Type type,
        DateTime createdAt,
        List<Template> templates)
    {
        AddNotification(name.NotNullOrEmptyOrWhiteSpace());
        AddNotification(name.BetweenLength(2, 50));

        if (Notifications.Count > 1)
        {
            return base.Validate();
        }

        Id = id;
        Name = name;
        Type = type;
        CreatedAt = createdAt;
        Templates = templates;

        var result = base.Validate();

        return result;
    }

    public static (DomainResult, Rule?) New(
        string name,
        ValueObjects.Type type,
        List<Template> templates)
    {
        var entity = new Rule();

        var response = entity.SetValues(RuleId.New(), name, type, DateTime.UtcNow, templates);

        return (response, entity);
    }
}
