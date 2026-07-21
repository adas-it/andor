using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Foundation.Domain.SeedWork.CommandRepository;

namespace Andor.Accounts.Domain.PaymentMethods.Repositories;

public interface ICommandsPaymentMethodRepository : ICommandRepository<PaymentMethod, PaymentMethodId>
{
    Task<IReadOnlyList<PaymentMethod>> GetTemplatesAsync(CancellationToken cancellationToken);
}
