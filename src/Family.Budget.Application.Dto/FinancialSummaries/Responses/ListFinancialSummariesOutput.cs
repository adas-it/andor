﻿namespace Family.Budget.Application.Dto.FinancialSummaries.Responses;

using Family.Budget.Application.Dto.Models.Response;
using System.Collections.Generic;
public record ListFinancialSummariesOutput
    : PaginatedListOutput<FinancialSummariesOutput>
{
    public ListFinancialSummariesOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<FinancialSummariesOutput> items)
        : base(page, perPage, total, items)
    {
    }
}