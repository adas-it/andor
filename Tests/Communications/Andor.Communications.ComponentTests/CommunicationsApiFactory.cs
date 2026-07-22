using Andor.Application.Communications.Interfaces;
using Andor.ComponentTests.Common;
using Andor.Communications.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Andor.Communications.ComponentTests;

public sealed class CommunicationsApiFactory : ComponentTestWebApplicationFactory<Program, CommunicationContext>
{
    public FakeSmtp Smtp { get; } = new();

    protected override void ConfigureAdditionalTestServices(IServiceCollection services)
    {
        services.RemoveAll(typeof(ISMTP));
        services.AddSingleton<ISMTP>(Smtp);
    }
}
