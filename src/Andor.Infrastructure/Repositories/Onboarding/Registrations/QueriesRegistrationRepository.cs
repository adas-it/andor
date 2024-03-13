using Andor.Domain.Entities.Onboarding.Registrations;
using Andor.Domain.Entities.Onboarding.Registrations.Repositories;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Andor.Infrastructure.Repositories.Onboarding.Registrations;

public class QueriesRegistrationRepository(PrincipalContext context) :
    QueryHelper<Registration, RegistrationId>(context),
    IQueriesRegistrationRepository
{
    public async Task<Registration?> GetByEmail(MailAddress email, CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Email.Equals(email), cancellationToken);

    public Task<SearchOutput<Registration>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
