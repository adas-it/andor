using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.ValueObjects;

namespace Andor.Domain.Engagement.Budget.Accounts.Accounts;

public class AccountPaymentMethod
{
    public AccountId AccountId { get; set; }
    public Account? Account { get; set; }
    public PaymentMethodId PaymentMethodId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}
