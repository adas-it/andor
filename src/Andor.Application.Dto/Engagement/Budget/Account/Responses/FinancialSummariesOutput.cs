namespace Andor.Application.Dto.Engagement.Budget.Account.Responses;

public record FinancialSummariesOutput
{
    public decimal Week1 { get; set; }
    public decimal Week2 { get; set; }
    public decimal Week3 { get; set; }
    public decimal Week4 { get; set; }
    public decimal Week5 { get; set; }
    public decimal Week6 { get; set; }
    public string Avatar { get; set; }
    public KeyValuePair<Guid, string> SubCategory { get; set; }
    public KeyValuePair<Guid, string> Category { get; set; }
    public KeyValuePair<int, string> CategoryType { get; set; }
}
