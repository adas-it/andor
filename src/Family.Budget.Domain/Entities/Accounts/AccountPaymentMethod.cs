namespace Family.Budget.Domain.Entities.Accounts;

using Family.Budget.Domain.Entities.PaymentMethods;

public class AccountPaymentMethod
{
    public AccountPaymentMethod()
    { }

    public AccountPaymentMethod(Account account, PaymentMethod paymentMethod)
    {
        Account = account;
        PaymentMethod = paymentMethod;
    }

    public Guid AccountId { get; set; }
    public Account Account { get; set; }
    public Guid PaymentMethodId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
