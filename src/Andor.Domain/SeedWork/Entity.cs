using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Validation;

namespace Andor.Domain.SeedWork;

public abstract class Entity<TEntityId> where TEntityId : IEquatable<TEntityId>
{
    public TEntityId Id { get; protected set; }

    protected readonly ICollection<Notification> _notifications;
    protected IReadOnlyCollection<Notification> Notifications => [.. _notifications];

    protected readonly ICollection<Notification> _warnings;
    protected IReadOnlyCollection<Notification> Warnings => [.. _warnings];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Entity()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _notifications = [];
        _warnings = [];
    }

    protected virtual DomainResult Validate()
    {
        AddNotification(Id!.NotNull());

        if (Notifications.Count != 0)
        {
            return DomainResult.Failure(errors: _notifications);
        }

        return DomainResult.Success(warnings: _warnings);
    }

    protected void AddNotification(Notification? notification)
    {
        if (notification != null)
        {
            _notifications.Add(notification);
        }
    }

    protected void AddNotification(string fieldName, string message, DomainErrorCode domainError)
        => AddNotification(new(fieldName, message, domainError));

    protected void AddWarning(Notification? notification)
    {
        if (notification != null)
        {
            _warnings.Add(notification);
        }
    }

    protected void AddWarning(string fieldName, string message, DomainErrorCode domainError)
        => AddWarning(new(fieldName, message, domainError));

}
