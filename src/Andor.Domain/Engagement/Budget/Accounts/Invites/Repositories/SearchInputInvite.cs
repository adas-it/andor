using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using System.Net.Mail;

namespace Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories
{
    public record SearchInputInvite(int Page, int PerPage, string? Search, string? OrderBy,
    SearchOrder Order, MailAddress email, AccountId AccountId)
    : SearchInput(Page, PerPage, Search, OrderBy, Order);
}
