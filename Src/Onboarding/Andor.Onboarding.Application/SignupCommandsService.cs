using Akka.Actor;
using Akka.Hosting;
using Andor.Authorizations.Domain;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.PasswordHasher;
using Andor.Onboarding.Application.Actors;
using Andor.Onboarding.Application.Commands;
using Andor.Onboarding.Application.Interfaces;
using Andor.Onboarding.Contracts.Responses;
using Andor.Onboarding.Domain;
using Andor.Onboarding.Domain.Repositories;
using Andor.Onboarding.Domain.ValueObjects;

namespace Andor.Onboarding.Application;

public class SignupCommandsService(ActorRegistry registry, ICommandsSignupRequestRepository repository, IPasswordHasher passwordHasher) : ISignupCommandsService
{
    private readonly IActorRef _signupActor = registry.Get<SignupManagerActor>();

    public async Task<ApplicationResult<object?>> StartSignupAsync(string name, string email, string preferredLanguage,
        ApplicationUser currentUser, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByEmailAsync(email, cancellationToken);
        var id = existing != null && !existing.IsVerified ? existing.Id : SignupRequestId.New();

        var command = new StartSignupCommand(id, name, email, preferredLanguage, currentUser, cancellationToken);

        return await Handler(command);
    }

    public async Task<ApplicationResult<object?>> VerifySignupAsync(string email, string code, string password,
        string preferredLanguage, bool marketingOptIn, bool termsAndConditionsAccepted, bool privacyPolicyAccepted,
        ApplicationUser currentUser, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<object?>.Success();

        var signupRequest = await repository.GetByEmailAsync(email, cancellationToken);

        if (signupRequest == null)
        {
            _ = response.AddError(SignupErrors.SignupNotFound());
            return response;
        }

        var passwordHash = passwordHasher.HashPassword(password);

        var command = new VerifySignupCommand(signupRequest.Id, code, passwordHash, preferredLanguage,
            marketingOptIn, termsAndConditionsAccepted, privacyPolicyAccepted, currentUser, cancellationToken);

        return await Handler(command);
    }

    private async Task<ApplicationResult<object?>> Handler(Andor.Foundation.Application.Commands.ICommands<SignupRequestId> command)
    {
        var response = ApplicationResult<object?>.Success();

        var (result, _) = await _signupActor.Ask<(DomainResult, SignupRequest?)>(command,
            command.CancellationToken);

        if (result.IsFailure)
        {
            await HandleSignupResult.HandleResultSignup(result, response);
        }

        return response;
    }
}
