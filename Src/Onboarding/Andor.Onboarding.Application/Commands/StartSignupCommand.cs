using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Domain.ValueObjects;

namespace Andor.Onboarding.Application.Commands;

public record StartSignupCommand(SignupRequestId Id, Name Name, string Email, string PreferredLanguage,
    ApplicationUser CurrentUser, CancellationToken CancellationToken) : ICommands<SignupRequestId>;
