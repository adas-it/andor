namespace Andor.Application.Dto.Engagement.Budget.Account.Responses;

public class CategorySummariesOutput
{
    public decimal Value { get; set; }
    public int Order { get; set; }
    public CategoryOutuput Category { get; set; }
    public KeyValuePair<int, string> CategoryType { get; set; }


    public record CategoryOutuput(Guid key, string value, int order);
}
