using Family.Budget.Application.Dto.Common.Response;

namespace Family.Budget.Application.Dto.FinancialSummaries.Responses;
public class FinancialSummariesOutput
{
    public decimal Week1 { get; set; }
    public decimal Week2 { get; set; }
    public decimal Week3 { get; set; }
    public decimal Week4 { get; set; }
    public decimal Week5 { get; set; }
    public decimal Week6 { get; set; }
    public string Avatar { get; set; }
    public KeyValuePairModel<Guid, string> SubCategory { get; set; }
    public KeyValuePairModel<Guid, string> Category { get; set; }
    public KeyValuePairModel<int, string> CategoryType { get; set; }
}
