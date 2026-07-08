using Andor.Assets.Domain.Investments.Areas.Repositories;
using Andor.Assets.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Assets.Binder.Infrastructure;

internal static class InfrastructureIoc
{
    internal static IServiceCollection WithConfigurationInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.WithInvestmentDbContext(configuration);

        services.AddTransient<ICommandsAreaRepository, CommandsAreaRepository>();

        return services;
    }
}
