using Andor.Onboarding.Domain;
using Andor.Onboarding.Domain.Repositories;
using Andor.Onboarding.Domain.ValueObjects;
using Andor.Onboarding.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Onboarding.Infrastructure;

public class CommandsSignupRequestRepository(OnboardingContext context) : ICommandsSignupRequestRepository
{
    protected readonly DbSet<SignupRequest> DbSet = context.Set<SignupRequest>();

    public Task<SignupRequest?> GetByIdAsync(SignupRequestId id, CancellationToken cancellationToken)
    {
        var entity = DbSet.FirstOrDefault(x => x.Id == id);

        return Task.FromResult<SignupRequest?>(entity);
    }

    public Task<SignupRequest?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var entity = DbSet.FirstOrDefault(x => x.Email == email);

        return Task.FromResult<SignupRequest?>(entity);
    }

    public async Task PersistAsync(SignupRequest entity, CancellationToken cancellationToken)
    {
        context.Upsert<SignupRequest, SignupRequestId>(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}
