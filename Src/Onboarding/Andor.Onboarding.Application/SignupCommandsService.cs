using Akka.Actor;
using Akka.Hosting;
using Andor.Authorizations.Domain;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Application.Actors;
using Andor.Onboarding.Application.Commands;
using Andor.Onboarding.Application.Interfaces;
using Andor.Onboarding.Contracts.Responses;
using Andor.Onboarding.Domain;
using Andor.Onboarding.Domain.Repositories;
using Andor.Onboarding.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Andor.Onboarding.Application;

public class SignupCommandsService(ActorRegistry registry, ICommandsSignupRequestRepository repository) : ISignupCommandsService
{
    private readonly IActorRef _signupActor = registry.Get<SignupManagerActor>();

    public async Task<ApplicationResult<object?>> StartSignupAsync(string name, string email,
        ApplicationUser currentUser, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByEmailAsync(email, cancellationToken);
        var id = existing != null && !existing.IsVerified ? existing.Id : SignupRequestId.New();

        var command = new StartSignupCommand(id, name, email, currentUser, cancellationToken);

        return await Handler(command);
    }

    public async Task<ApplicationResult<object?>> VerifySignupAsync(string email, string code, string password,
        ApplicationUser currentUser, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<object?>.Success();

        var signupRequest = await repository.GetByEmailAsync(email, cancellationToken);

        if (signupRequest == null)
        {
            response.AddError(SignupErrors.SignupNotFound());
            return response;
        }

        // The generic user-type parameter is never dereferenced by PasswordHasher<T>'s internal
        // logic, so it's safe to hash here without depending on Identity's ApplicationUser type.
        // Only the resulting hash — never the raw password — flows through the command/event.
        var passwordHash = new PasswordHasher<object>().HashPassword(null!, password);

        var command = new VerifySignupCommand(signupRequest.Id, code, passwordHash, currentUser, cancellationToken);

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
