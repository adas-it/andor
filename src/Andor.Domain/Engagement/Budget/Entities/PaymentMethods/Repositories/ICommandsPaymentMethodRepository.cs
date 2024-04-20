using Andor.Domain.Engagement.Budget.Entities.PaymentMethods;
using Andor.Domain.Engagement.Budget.Entities.PaymentMethods.ValueObjects;
using Andor.Domain.SeedWork.Repository.CommandRepository;

namespace Andor.Domain.Onboarding.Registrations.Repositories;

public interface ICommandsPaymentMethodRepository : ICommandRepository<PaymentMethod, PaymentMethodId>
{
}
