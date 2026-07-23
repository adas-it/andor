namespace Andor.Accounts.Contracts.Responses;

public record FinancialSummariesOutput
{
    public decimal Week1 { get; set; }
    public decimal Week2 { get; set; }
    public decimal Week3 { get; set; }
    public decimal Week4 { get; set; }
    public decimal Week5 { get; set; }
    public SummariesOutput? SubCategory { get; set; }
    public SummariesOutput? Category { get; set; }
    public KeyValuePair<int, string> CategoryType { get; set; }

    public record SummariesOutput(string Key, string Value, int Order);
}
