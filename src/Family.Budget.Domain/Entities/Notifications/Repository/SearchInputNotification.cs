namespace Family.Budget.Domain.Entities.Notifications.Repository;

using Family.Budget.Domain.SeedWork.ShearchableRepository;
using System;

public record SearchInputNotification : SearchInput
{
    public SearchInputNotification(int Page, int PerPage, string? Search, string? OrderBy, SearchOrder Order) : base(Page, PerPage, Search, OrderBy, Order)
    {
    }

    public Guid UserId { get; set; }
}
