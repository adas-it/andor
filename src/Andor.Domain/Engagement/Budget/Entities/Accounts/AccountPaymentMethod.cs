using Andor.Domain.Engagement.Budget.Entities.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Entities.PaymentMethods;
using Andor.Domain.Engagement.Budget.Entities.PaymentMethods.ValueObjects;

namespace Andor.Domain.Engagement.Budget.Entities.Accounts;

public class AccountPaymentMethod
{
    public AccountId AccountId { get; set; }
    public Account? Account { get; set; }
    public PaymentMethodId PaymentMethodId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}
