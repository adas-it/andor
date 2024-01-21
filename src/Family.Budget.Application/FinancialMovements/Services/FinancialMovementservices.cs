namespace Family.Budget.Application.FinancialMovements.Services;

using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.FinancialMovement;
using Family.Budget.Domain.Entities.FinancialMovement.Repository;
using System.Linq;
using System.Threading.Tasks;

public class FinancialMovementservices : IFinancialMovementservices
{
    private readonly IFinancialMovementRepository FinancialMovementRepository;
    private readonly Notifier notifier;

    public FinancialMovementservices(IFinancialMovementRepository FinancialMovementRepository,
        Notifier notifier)
    {
        this.FinancialMovementRepository = FinancialMovementRepository;
        this.notifier = notifier;
    }

    public Task Handle(FinancialMovement entity, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
