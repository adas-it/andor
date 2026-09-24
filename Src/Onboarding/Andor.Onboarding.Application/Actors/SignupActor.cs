using Akka.Actor;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Application.Commands;
using Andor.Onboarding.Application.Interfaces;
using Andor.Onboarding.Domain;
using Andor.Onboarding.Domain.Errors;
using Andor.Onboarding.Domain.Repositories;
using Andor.Onboarding.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Onboarding.Application.Actors;

public class SignupActor : ReceiveActor, IWithUnboundedStash
{
    private readonly SignupRequestId _id;
    private SignupRequest? _signupRequest;
    private readonly IServiceProvider _serviceProvider;

    public IStash? Stash { get; set; }

    public SignupActor(SignupRequestId id, IServiceProvider serviceProvider)
    {
        _id = id;
        _serviceProvider = serviceProvider;

        Become(Loading);
    }

    protected override void PreStart()
    {
        Self.Tell(new PreLoadSignupRequest(_id));
        base.PreStart();
    }

    private void Loading()
    {
        ReceiveAsync<PreLoadSignupRequest>(async _ =>
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsSignupRequestRepository>();

            var result = await repo.GetByIdAsync(_id, CancellationToken.None);

            if (result == null)
                return;

            _signupRequest = result;

            Become(Ready);
            Stash!.UnstashAll();
        });

        ReceiveAsync<StartSignupCommand>(HandleStartAsync);

        ReceiveAny(_ => Stash!.Stash());
    }

    private async Task HandleStartAsync(StartSignupCommand cmd)
    {
        using var scope = _serviceProvider.CreateScope();

        var validator = scope.ServiceProvider.GetRequiredService<IOnboardingValidator>();
        var repo = scope.ServiceProvider.GetRequiredService<ICommandsSignupRequestRepository>();

        var (domainResult, signupRequest) = await SignupRequest.NewAsync(
            _id,
            cmd.Name,
            cmd.Email,
            cmd.PreferredLanguage,
            validator,
            cmd.CancellationToken);

        if (signupRequest?.Events.Count > 0)
            await repo.PersistAsync(signupRequest, cmd.CancellationToken);

        _signupRequest = signupRequest;
        Sender.Tell((domainResult, signupRequest));

        if (signupRequest != null)
        {
            Become(Ready);
            Stash!.UnstashAll();
        }
    }

    private void Ready()
    {
        ReceiveAsync<VerifySignupCommand>(HandleVerifyAsync);
        ReceiveAsync<StartSignupCommand>(HandleRestartAsync);
    }

    private async Task HandleRestartAsync(StartSignupCommand cmd)
    {
        using var scope = _serviceProvider.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<IOnboardingValidator>();
        var repo = scope.ServiceProvider.GetRequiredService<ICommandsSignupRequestRepository>();

        var result = await _signupRequest!.RestartAsync(cmd.Name, cmd.PreferredLanguage, validator, cmd.CancellationToken);

        if (result.IsSuccess && _signupRequest.Events.Count > 0)
            await repo.PersistAsync(_signupRequest, cmd.CancellationToken);

        Sender.Tell((result, _signupRequest));
    }

    private async Task HandleVerifyAsync(VerifySignupCommand cmd)
    {
        if (_signupRequest is null)
        {
            DomainResult notFound = DomainResult.Failure(
                errors: new[] {
                    new Notification(nameof(_id), $"Signup request '{_id}' not found", SignupErrorCodes.SignupNotFound)
                });

            Sender.Tell((notFound, (SignupRequest?)null));
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICommandsSignupRequestRepository>();

        // User/Identity/Account provisioning is no longer a synchronous dependency of Verify: it
        // happens out-of-band, triggered by consumers reacting to the SignupVerifiedDomainEvent
        // raised below. The id is minted here so it's the one the aggregate (and thus the event)
        // carries, and Users.Service creates its User row with this same id.
        var userId = Guid.NewGuid();

        var result = _signupRequest.Verify(cmd.Code, cmd.PasswordHash, userId, cmd.PreferredLanguage,
            cmd.PreferredCurrency, cmd.MarketingOptIn, cmd.TermsAndConditionsAccepted, cmd.PrivacyPolicyAccepted);

        if (result.IsSuccess)
        {
            await repo.PersistAsync(_signupRequest, cmd.CancellationToken);
        }

        Sender.Tell((result, _signupRequest));
    }

    private record PreLoadSignupRequest(SignupRequestId Id);
}
