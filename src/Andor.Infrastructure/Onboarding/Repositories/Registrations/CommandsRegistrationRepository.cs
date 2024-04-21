using Andor.Domain.Administrations.Languages;
using Andor.Domain.Engagement.Budget.Accounts.Currencies;
using Andor.Domain.Onboarding.Registrations;
using Andor.Domain.Onboarding.Registrations.Repositories;
using Andor.Domain.Onboarding.Registrations.ValueObjects;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Infrastructure.Onboarding.Repositories.Registrations;

public class CommandsRegistrationRepository(PrincipalContext context) :
    CommandsBaseRepository<Registration, RegistrationId>(context),
ICommandsRegistrationRepository
{
    protected readonly DbSet<Currency> _dbCurrencySet = context.Set<Currency>();
    protected readonly DbSet<Language> _dbLanguageSet = context.Set<Language>();

    public override Task InsertAsync(Registration entity, CancellationToken cancellationToken)
    {
        _dbLanguageSet.Attach(entity.Language);
        _dbCurrencySet.Attach(entity.Currency);
        return base.InsertAsync(entity, cancellationToken);
    }
}
