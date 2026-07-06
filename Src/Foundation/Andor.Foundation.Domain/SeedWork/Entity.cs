using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Foundation.Domain.SeedWork;

public abstract class Entity<TEntityId> where TEntityId : IEquatable<TEntityId>, IId<TEntityId>
{
    public TEntityId Id { get; protected init; } = TEntityId.New();

    private readonly ICollection<Notification> _notifications = [];
    protected IReadOnlyCollection<Notification> Notifications => [.. _notifications];

    private readonly ICollection<Notification> _warnings = [];
    protected IReadOnlyCollection<Notification> Warnings => [.. _warnings];

    private readonly ICollection<Notification> _infos = [];
    protected IReadOnlyCollection<Notification> Information => [.. _infos];

    protected virtual DomainResult Validate()
    {
        AddNotification(Id!.NotNull());

        return Notifications.Count != 0
            ? DomainResult.Failure(errors: _notifications)
            : DomainResult.Success(warnings: _warnings);
    }

    protected void AddNotification(List<Notification> notifications)
    {
        notifications.ForEach(x => AddNotification(x));
    }

    protected void AddNotification(Notification? notification)
    {
        if (notification != null)
        {
            _notifications.Add(notification);
        }
    }

    protected void AddNotification(string fieldName, string message, DomainErrorCode domainError)
        => AddNotification(new Notification(fieldName, message, domainError));

    protected void AddWarning(Notification? notification)
    {
        if (notification != null)
        {
            _warnings.Add(notification);
        }
    }

    protected void AddWarning(string fieldName, string message, DomainErrorCode domainError)
        => AddWarning(new(fieldName, message, domainError));

    protected void AddInformation(Notification? notification)
    {
        if (notification != null)
        {
            _infos.Add(notification);
        }
    }

    protected void AddInformation(string fieldName, string message, DomainErrorCode domainError)
        => AddInformation(new(fieldName, message, domainError));

    protected async Task<DomainResult> ValidateAsync<TREntity>(
        IDefaultValidator<TREntity, TEntityId> validator,
        CancellationToken cancellationToken)
        where TREntity : Entity<TEntityId>
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (this is not TREntity entity)
            throw new InvalidOperationException($"Entity is not of type {typeof(TREntity).Name}");

        var notifications = await validator.ValidateCreationAsync(entity, cancellationToken);

        AddNotification(notifications);
        AddNotification(Id!.NotNull());

        var domainResult = Notifications.Count != 0
            ? DomainResult.Failure(errors: _notifications)
            : DomainResult.Success(warnings: _warnings);

        return domainResult;
    }
}
