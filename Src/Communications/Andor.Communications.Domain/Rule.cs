using Andor.Communications.Domain.Errors;
using Andor.Communications.Domain.Events;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Domain;

/// <summary>
/// Represents a communication rule aggregate root that groups the templates used to
/// deliver a given type of message.
/// </summary>
public class Rule : AggregateRoot<RuleId>
{
    /// <summary>
    /// Gets the name of the rule.
    /// </summary>
    public Name Name { get; private set; }

    /// <summary>
    /// Gets the type of the rule.
    /// </summary>
    public ValueObjects.Type Type { get; private set; }

    /// <summary>
    /// Gets the date and time when the rule was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the templates associated with the rule.
    /// </summary>
    public ICollection<Template> Templates { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rule"/> class.
    /// Default parameterless constructor for ORM usage.
    /// </summary>
    private Rule()
    {
        Name = string.Empty;
        Type = ValueObjects.Type.Undefined;
        Templates = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rule"/> class with the specified values.
    /// </summary>
    /// <param name="id">The unique identifier for the rule.</param>
    /// <param name="name">The name of the rule.</param>
    /// <param name="type">The type of the rule.</param>
    /// <param name="createdAt">The date and time when the rule was created.</param>
    /// <param name="templates">The templates associated with the rule.</param>
    private Rule(
        RuleId id,
        Name name,
        ValueObjects.Type type,
        DateTime createdAt,
        List<Template> templates)
    {
        Id = id;
        Name = name;
        Type = type;
        CreatedAt = createdAt;
        Templates = templates;
    }

    /// <summary>
    /// Creates a new rule instance asynchronously with validation.
    /// </summary>
    /// <param name="name">The name of the rule.</param>
    /// <param name="type">The type of the rule.</param>
    /// <param name="templates">The templates associated with the rule.</param>
    /// <param name="skipValidations">If true, skips validation rules.</param>
    /// <param name="userId">The identifier of the user creating the rule.</param>
    /// <param name="validator">The validator to use for validation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A tuple containing the domain result and the created rule if successful.</returns>
    public static async Task<(DomainResult, Rule?)> NewAsync(
        RuleId id,
        string name,
        ValueObjects.Type type,
        List<Template> templates,
        bool skipValidations,
        Guid userId,
        IRuleValidator validator,
        CancellationToken cancellationToken)
    {
        DomainResult result;

        var entity = new Rule(
            id,
            name,
            type,
            DateTime.UtcNow,
            templates);

        if (skipValidations)
        {
            result = DomainResult.Success(infos: new List<Notification>()
            {
                new Notification("", "All Validations has been skipped.", CommunicationsErrorCodes.SkippedValidations)
            });
        }
        else
        {
            result = await entity.ValidateAsync(validator, cancellationToken);
        }

        if (result.IsSuccess && entity != null)
        {
            entity.RaiseDomainEvent(RuleCreated.FromRule(entity, userId));
        }

        return (result, entity);
    }
}
