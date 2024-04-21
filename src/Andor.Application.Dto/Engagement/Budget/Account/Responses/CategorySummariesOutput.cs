namespace Andor.Application.Dto.Engagement.Budget.Account.Responses;

public class CategorySummariesOutput
{
    public decimal Value { get; set; }
    public KeyValuePair<Guid, string> Category { get; set; }
    public KeyValuePair<int, string> CategoryType { get; set; }
}
