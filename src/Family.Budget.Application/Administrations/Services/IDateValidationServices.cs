namespace Family.Budget.Application.Administrations.Services;

using System.Threading.Tasks;
using Family.Budget.Domain.Entities.Admin;

public interface IDateValidationServices
{
    Task Handle(Configuration entity, CancellationToken cancellationToken);
}
