using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.PaymentMethods;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;

namespace Andor.Accounts.Domain.Accounts;

public class AccountPaymentMethod
{
    public AccountPaymentMethod()
    {
    }

    public AccountPaymentMethod(Account account, PaymentMethod paymentMethod, int order)
    {
        Account = account;
        AccountId = account.Id;
        PaymentMethod = paymentMethod;
        PaymentMethodId = paymentMethod.Id;
        Order = order;
    }

    public AccountId AccountId { get; set; }
    public Account Account { get; set; }
    public PaymentMethodId PaymentMethodId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public int Order { get; set; }
}
