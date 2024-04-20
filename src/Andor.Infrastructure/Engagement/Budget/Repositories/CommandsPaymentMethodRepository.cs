using Andor.Domain.Engagement.Budget.Entities.PaymentMethods;
using Andor.Domain.Engagement.Budget.Entities.PaymentMethods.ValueObjects;
using Andor.Domain.Onboarding.Registrations.Repositories;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class CommandsPaymentMethodRepository(PrincipalContext context) :
    CommandsBaseRepository<PaymentMethod, PaymentMethodId>(context),
    ICommandsPaymentMethodRepository
{
}
