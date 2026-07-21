using Andor.Foundation.Domain.SeedWork.CommandRepository;
using Andor.Onboarding.Domain.ValueObjects;

namespace Andor.Onboarding.Domain.Repositories;

public interface ICommandsSignupRequestRepository : ICommandRepository<SignupRequest, SignupRequestId>
{
    Task<SignupRequest?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
