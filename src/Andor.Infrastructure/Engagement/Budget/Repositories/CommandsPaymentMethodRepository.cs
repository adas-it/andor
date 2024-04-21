using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;

namespace Andor.Infrastructure.Engagement.Budget.Repositories;

public class CommandsPaymentMethodRepository(PrincipalContext context) :
    CommandsBaseRepository<PaymentMethod, PaymentMethodId>(context),
    ICommandsPaymentMethodRepository
{
}
