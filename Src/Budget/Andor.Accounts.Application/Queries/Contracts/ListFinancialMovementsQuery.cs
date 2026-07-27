using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Foundation.Application.Queries;

namespace Andor.Accounts.Application.Queries.Contracts;

public record ListFinancialMovementsQuery : SearchInput
{
    public AccountId AccountId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}
