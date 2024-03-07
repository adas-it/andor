namespace Andor.Domain.Common;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; }
}
