namespace Family.Budget.Application.FinancialMovements.Services;

using System.Threading.Tasks;
using Family.Budget.Domain.Entities.FinancialMovement;

public interface IFinancialMovementservices
{
    Task Handle(FinancialMovement entity, CancellationToken cancellationToken);
}
