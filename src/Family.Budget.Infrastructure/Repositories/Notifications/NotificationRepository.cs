namespace Family.Budget.Infrastructure.Repositories.Notifications;
using Family.Budget.Domain.Entities.Notifications;
using Family.Budget.Domain.Entities.Notifications.Repository;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

public class NotificationRepository : QueryHelper<Notification>, INotificationsRepository
{
    public NotificationRepository(PrincipalContext context) : base(context)
    {
    }
    public async Task Insert(Notification entity, CancellationToken cancellationToken)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task Update(Notification entity, CancellationToken cancellationToken)
    {
        _dbSet.Attach(entity);
        _dbSet.Update(entity);

        return Task.CompletedTask;
    }

    public Task Delete(Notification entity, CancellationToken cancellationToken)
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

    public async Task<Notification?> GetById(Guid id, CancellationToken cancellationToken)
    => await _dbSet
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<SearchOutput<Notification>> Search(SearchInputNotification input, CancellationToken cancellationToken)
    {
        Expression<Func<Notification, bool>> where = x => x.UserId == input.UserId;

        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            where = x => x.Description.ToLower().Contains(input.Search.ToLower()) &&
            x.UserId == input.UserId;
        }

        var items = GetManyPagined(where,
            input.OrderBy,
            input.Order,
            input.Page,
            input.PerPage,
            out var total)
            .ToList();

        return Task.FromResult(new SearchOutput<Notification>(input.Page, input.PerPage, total, items!));
    }
}
