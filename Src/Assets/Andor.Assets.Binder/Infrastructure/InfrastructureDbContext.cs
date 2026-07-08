using Andor.Assets.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Assets.Binder.Infrastructure;

internal static class InfrastructureDbContext
{
    internal static IServiceCollection WithInvestmentDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<InvestmentContext>();

        return services;
    }
}
