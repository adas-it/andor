using Andor.Domain.Administrations.Languages.ValueObjects;
using Andor.Domain.Common.ValuesObjects;
using Andor.Domain.Engagement.Budget.Accounts.Currencies.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Users.DomainEvents;
using Andor.Domain.Engagement.Budget.Accounts.Users.ValueObjects;
using Andor.Domain.SeedWork;
using System.Net.Mail;

namespace Andor.Domain.Engagement.Budget.Accounts.Users
{
    public sealed class User : AggregateRoot<UserId>
    {
        public UserId Id { get; private set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public MailAddress Email { get; private set; }
        public CurrencyId PreferredCurrencyId { get; set; }
        public LanguageId PreferredLanguageId { get; set; }

        private DomainResult SetValues(UserId id,
            MailAddress email,
            string firstName,
            string lastName,
            CurrencyId preferredCurrencyId,
            LanguageId preferredLanguageId)
        {

            if (Notifications.Count > 1)
            {
                return Validate();
            }

            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PreferredCurrencyId = preferredCurrencyId;
            PreferredLanguageId = preferredLanguageId;

            var result = Validate();

            return result;
        }

        public static (DomainResult, User?) New(UserId userId, MailAddress email,
            string firstName,
            string lastName,
            CurrencyId preferredCurrencyId,
            LanguageId preferredLanguageId)
        {
            var entity = new User();

            var result = entity.SetValues(userId,
                email,
                firstName,
                lastName,
                preferredCurrencyId,
               preferredLanguageId);

            if (result.IsFailure)
            {
                return (result, null);
            }

            entity.RaiseDomainEvent(UserCreatedDomainEvent.FromAggregateRoot(entity));

            return (result, entity);
        }
    }
}
