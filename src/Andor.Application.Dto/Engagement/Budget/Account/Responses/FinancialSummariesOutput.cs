namespace Andor.Application.Dto.Engagement.Budget.Account.Responses;

public record FinancialSummariesOutput
{
    public decimal Week1 { get; set; }
    public decimal Week2 { get; set; }
    public decimal Week3 { get; set; }
    public decimal Week4 { get; set; }
    public decimal Week5 { get; set; }
    public CategorySummarieOutuput SubCategory { get; set; }
    public CategorySummarieOutuput Category { get; set; }
    public KeyValuePair<int, string> CategoryType { get; set; }

    public record CategorySummarieOutuput(Guid key, string value, int order);
}
