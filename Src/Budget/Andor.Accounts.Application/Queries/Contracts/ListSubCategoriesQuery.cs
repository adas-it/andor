using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Categories.ValueObjects;
using Andor.Foundation.Application.Queries;

namespace Andor.Accounts.Application.Queries.Contracts;

public record ListSubCategoriesQuery : SearchInput
{
    public AccountId AccountId { get; set; }
    public CategoryId CategoryId { get; set; }
}
