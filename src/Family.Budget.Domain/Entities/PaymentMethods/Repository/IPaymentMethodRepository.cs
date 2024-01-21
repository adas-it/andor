namespace Family.Budget.Domain.Entities.PaymentMethods.Repository;

using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Domain.SeedWork.ShearchableRepository;

public interface IPaymentMethodRepository : IRepository<PaymentMethod>
{
    Task<List<PaymentMethod>> GetByName(string name, CancellationToken cancellationToken);
    Task<List<PaymentMethod>> GetByIds(List<Guid> ids, CancellationToken cancellationToken);
    Task<SearchOutput<PaymentMethod>> Search(SearchInput input, MovementType type, CancellationToken cancellationToken);
}
