using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.ValueObjects;
using Andor.Domain.SeedWork.Repositories.CommandRepository;

namespace Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.Repositories;

public interface ICommandsPaymentMethodRepository : ICommandRepository<PaymentMethod, PaymentMethodId>
{
}
