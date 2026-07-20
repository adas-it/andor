using Andor.Application.Communications.Interfaces;
using Andor.Application.Communications.Services.Manager;
using Andor.Application.Communications.Services.PartnerHandler;
using Andor.Communications.Application;
using Andor.Communications.Application.Interfaces;
using Andor.Communications.Domain;
using Andor.Foundation.Binder;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Communications.Binder.Application;

internal static class ApplicationIoc
{
    public static IServiceCollection WithCommunicationApplication(this IServiceCollection services)
    {
        services.AddSingleton<IAkkaModule, ApplicationAkkaModule>();

        services.AddScoped<IRuleValidator, RuleValidator>();

        services.AddScoped<IPartner, InHousePartner>();
        services.AddScoped<IPartnerManager, PartnerManager>();

        services.AddScoped<IRuleCommandsService, RuleCommandsService>();

        return services;
    }
}
