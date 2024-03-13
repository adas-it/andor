namespace Family.Budget.Application.Dto.FinancialSummaries.Requests;

using Family.Budget.Application.Dto.Common.Request;
using System;
public record SearchSummaryInput(Guid AccountId, int Year, int MonthOfYear, int Page, int PerPage, string? OrderBy, SearchOrder Order);
