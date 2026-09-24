using Andor.Accounts.Domain.Accounts.Errors;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.Currencies;
using Andor.Accounts.Domain.PermissionTypes;
using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Accounts.Domain.Accounts;

public class AccountValidator()
    : DefaultValidator<Account, AccountId>, IAccountValidator
{
    public override async Task<List<Notification>> ValidateCreationAsync(Account entity,
        CancellationToken cancellationToken)
    {
        List<Notification> notifications = [];

        AddNotification(entity.Name.NotNull(), notifications);
        AddNotification(entity.Description.NotNull(), notifications);
        AddNotification(entity.Currency.NotNull(), notifications);

        if (!entity.Members.Any(x => x.PermissionType == PermissionType.Owner))
        {
            notifications.Add(
                new Notification(nameof(entity.Members),
                AccountErrorMessages.AccountShouldHaveOneOwner, AccountErrorCode.AccountShouldHaveOneOwner));
        }

        return notifications;

    }

    public Task<List<Notification>> ValidateUpdateAsync(Account account, Name name, Description description,
        Currency currency, CancellationToken cancellationToken)
    {
        List<Notification> notifications = [];

        if (name == null)
        {
            AddNotification(new Notification(nameof(account.Name),
                AccountErrorMessages.NameCannotBeNull, AccountErrorCode.NameCannotBeNull),
                notifications);
        }

        if (currency == null)
        {
            AddNotification(new Notification(nameof(account.Name),
                AccountErrorMessages.CurrencyCannotBeNull, AccountErrorCode.CurrencyCannotBeNull),
                notifications);
        }

        return Task.FromResult(notifications);
    }
}
