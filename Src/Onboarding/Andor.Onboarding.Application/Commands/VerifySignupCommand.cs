using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Commands;
using Andor.Onboarding.Domain.ValueObjects;

namespace Andor.Onboarding.Application.Commands;

// PasswordHash is already hashed by SignupCommandsService before this command is built —
// the raw password never reaches the actor system or gets published on any event.
public record VerifySignupCommand(SignupRequestId Id, string Code, string PasswordHash,
    ApplicationUser CurrentUser, CancellationToken CancellationToken) : ICommands<SignupRequestId>;
