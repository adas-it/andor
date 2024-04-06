using Andor.Domain.Entities.Onboarding.Registrations.ValueObjects;
using Andor.Domain.SeedWork.Repository.ISearchableRepository;
using System.Net.Mail;

namespace Andor.Domain.Entities.Onboarding.Registrations.Repositories;

public interface IQueriesRegistrationRepository :
    IResearchableRepository<Registration, RegistrationId, SearchInput>
{
    Task<Registration?> GetByEmailAsync(MailAddress email, CancellationToken cancellationToken);
}
