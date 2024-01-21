using Family.Budget.Application.Dto.Notifications.NotificationType;
using Family.Budget.Domain.Entities.Notifications.NotificationTypes;
using Mapster;
using MediatR;

namespace Family.Budget.Application.Notifications.Queries;

public class ListNotificationTypesQuery
: IRequest<List<NotificationTypeOutput>>
{
}

public class ListNotificationTypesQueryHandler : IRequestHandler<ListNotificationTypesQuery, List<NotificationTypeOutput>>
{
    public Task<List<NotificationTypeOutput>> Handle(ListNotificationTypesQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(
            NotificationType.GetAll<NotificationType>().ToList().Adapt<List<NotificationTypeOutput>>());
    }
}
