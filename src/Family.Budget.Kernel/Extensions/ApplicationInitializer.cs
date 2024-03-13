namespace Family.Budget.Kernel.Extensions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public static class ApplicationInitializer
{
    private static readonly Assembly DomainAssembly;
    private static readonly Assembly InfrastructureAssembly;
    private static readonly Assembly ApplicationAssembly;

    static ApplicationInitializer()
    {
        DomainAssembly = AppDomain.CurrentDomain.Load("Family.Budget.Domain");
        ApplicationAssembly = AppDomain.CurrentDomain.Load("Family.Budget.Application");
        InfrastructureAssembly = AppDomain.CurrentDomain.Load("Family.Budget.Infrastructure");
    }

    public static IServiceCollection ConfigureApplicationInfrastructure(this IServiceCollection services)
    {
        var assembly1 = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly1));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(DomainAssembly));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(InfrastructureAssembly));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ApplicationAssembly));

        return services;
    }
}
