namespace Family.Budget.Infrastructure.Repositories.SubCategories;

using Family.Budget.Domain.Entities.Accounts;
using Family.Budget.Domain.Entities.Accounts.Repository;
using Family.Budget.Domain.Entities.Accounts.ValueObject;
using Family.Budget.Domain.Entities.Users;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public class AccountRepository : QueryHelper<Account>, IAccountRepository
{
    private readonly PrincipalContext _context;
    public AccountRepository(PrincipalContext context) : base(context)
    {
        _context = context;
    }
    public async Task Insert(Account entity, CancellationToken cancellationToken)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public async Task Update(Account entity, CancellationToken cancellationToken)
    {
        foreach (var invite in entity.Invites)
        {
            if (!_context.Invite.Any(x => x.Id == invite.Id))
            {
                _context.Invite.Add(invite);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _context.Update(entity);
    }

    public Task Delete(Account entity, CancellationToken cancellationToken)
        => Task.FromResult(_dbSet.Remove(entity));

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var ids = new object[] { id };
        var item = await _dbSet.FindAsync(ids, cancellationToken);

        if (item != null)
        {
            _dbSet.Remove(item);
        }
    }

    public async Task<Account?> GetById(Guid id, CancellationToken cancellationToken)
        => await _dbSet
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<SearchOutput<Account>> Search(SearchInputAccount input, CancellationToken cancellationToken)
    {
        Expression<Func<Account, bool>> where = x => true;

        if (!string.IsNullOrWhiteSpace(input.Search))
            where = x => x.Name.ToLower().Contains(input.Search.ToLower());

        where = x => x.UserIds.Any(z => z.UserId == (Guid)input.UserId);

        var items = GetManyPagined(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Account>(input.Page, input.PerPage, total, items!));
    }

    public async Task<Account?> GetByIdandUser(AccountId id, UserId UserId, CancellationToken cancellationToken)
    => await _dbSet
        .Include(x => x.UserIds)
        .Include(x => x.SubCategories).ThenInclude(z => z.SubCategory).ThenInclude(x => x.Category)
        .Include(x => x.PaymentMethods).ThenInclude(z => z.PaymentMethod).Include(x => x.Categories)
        .FirstOrDefaultAsync(x => x.Id == (Guid)id && x.UserIds.Any(x => x.UserId == (Guid)UserId), cancellationToken);
}
