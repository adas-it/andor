namespace Family.Budget.Domain.Entities.FinancialMovement.Repository;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.SeedWork.ShearchableRepository;

public interface IFinancialMovementRepository : IRepository<FinancialMovement>, ISearchableRepository<FinancialMovement, SearchInputMovement>
{
    Task<List<FinancialMovement>> GetAllFinancialMovementsByMonth(
        Guid accountId,
        int year,
        int month,
        CancellationToken cancellationToken);

    Task<DateTime?> GetFirstMovement(Guid accountId, 
        CancellationToken cancellationToken);

    Task<DateTime?> GetLastMovement(Guid accountId, 
        CancellationToken cancellationToken);
}
