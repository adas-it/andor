namespace Andor.Foundation.Domain;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; }
}
