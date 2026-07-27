namespace Andor.Accounts.Contracts.Accounts.Responses;

public class CategorySummariesOutput
{
    public decimal Value { get; set; }
    public int Order { get; set; }
    public CategoryKeyOutput? Category { get; set; }
    public KeyValuePair<int, string> CategoryType { get; set; }


    public record CategoryKeyOutput(string Key, string Value, int Order);
}
