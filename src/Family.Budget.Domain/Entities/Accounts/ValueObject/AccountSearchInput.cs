namespace Family.Budget.Domain.Entities.Accounts.ValueObject;

using Family.Budget.Domain.Common.ValuesObjects;
using Family.Budget.Domain.Entities.Users;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
public record SearchInputAccount(
    int Page, 
    int PerPage, 
    string? Search, 
    string? OrderBy, 
    SearchOrder Order, 
    AccountId? AccountId,
    UserId UserId,
    Year? Year,
    Month? Month) 
    : SearchInput(Page, PerPage, Search, OrderBy, Order);