namespace Family.Budget.Domain.Entities.Notifications.Repository;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.SeedWork.ShearchableRepository;

public interface INotificationsRepository : IRepository<Notification>, ISearchableRepository<Notification, SearchInputNotification>
{
}
