namespace Family.Budget.Application.PaymentMethod.Services;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.PaymentMethods;
using Family.Budget.Domain.Entities.PaymentMethods.Repository;
using System.Linq;
using System.Threading.Tasks;

public class PaymentMethodServices : IPaymentMethodServices
{
    private readonly IPaymentMethodRepository PaymentMethodRepository;
    private readonly Notifier notifier;

    public PaymentMethodServices(IPaymentMethodRepository PaymentMethodRepository,
        Notifier notifier)
    {
        this.PaymentMethodRepository = PaymentMethodRepository;
        this.notifier = notifier;
    }

    public async Task Handle(PaymentMethod entity, CancellationToken cancellationToken)
    {
        var listWithSameName = await PaymentMethodRepository.GetByName(entity.Name, cancellationToken);

        if (listWithSameName is not null && listWithSameName.Where(x => x.Id != entity.Id).Any())
        {
        }
    }
}
