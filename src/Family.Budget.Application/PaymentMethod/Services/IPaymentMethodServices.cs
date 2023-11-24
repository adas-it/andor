namespace Family.Budget.Application.PaymentMethod.Services;

using System.Threading.Tasks;
using Family.Budget.Domain.Entities.PaymentMethods;

public interface IPaymentMethodServices
{
    Task Handle(PaymentMethod entity, CancellationToken cancellationToken);
}
