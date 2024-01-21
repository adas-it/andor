using Family.Budget.Application.Dto.Common.Response;

namespace Family.Budget.Application.Dto.FinancialSummaries.Responses;
public class CategorySummariesOutput
{
    public decimal Value { get; set; }
    public KeyValuePairModel<Guid, string> Category { get; set; }
    public KeyValuePairModel<int, string> CategoryType { get; set; }
}
