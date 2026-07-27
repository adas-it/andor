using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Foundation.Application.Queries;

namespace Andor.Accounts.Application.Queries.Contracts;

public record ListCategoriesQuery : SearchInput
{
    public AccountId AccountId { get; set; }
    public int? Type { get; set; }
}
