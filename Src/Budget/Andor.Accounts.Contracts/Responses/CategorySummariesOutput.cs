namespace Andor.Accounts.Contracts.Responses;

public class CategorySummariesOutput
{
    public decimal Value { get; set; }
    public int Order { get; set; }
    public CategoryOutput? Category { get; set; }
    public KeyValuePair<int, string> CategoryType { get; set; }


    public record CategoryOutput(string Key, string Value, int Order);
}
