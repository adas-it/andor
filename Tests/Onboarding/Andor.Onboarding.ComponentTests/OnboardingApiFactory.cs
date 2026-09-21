using Andor.ComponentTests.Common;
using Andor.Onboarding.Application.Interfaces;
using Andor.Onboarding.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Andor.Onboarding.ComponentTests;

public sealed class OnboardingApiFactory : ComponentTestWebApplicationFactory<Program, OnboardingContext>
{
    public FakeUserProvisioningClient UserProvisioning { get; } = new();

    protected override void ConfigureAdditionalTestServices(IServiceCollection services)
    {
        services.RemoveAll(typeof(IUserProvisioningClient));
        services.AddSingleton<IUserProvisioningClient>(UserProvisioning);
    }
}
