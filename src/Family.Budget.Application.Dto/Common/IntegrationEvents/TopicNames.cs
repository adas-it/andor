namespace Family.Budget.Application.Dto.Models.Events;
public record TopicNames
{
    protected TopicNames(string value) { Value = value; }

    public string Value { get; init; }

    public readonly static TopicNames Family_Budget  = new ("Family.Budget");
}
