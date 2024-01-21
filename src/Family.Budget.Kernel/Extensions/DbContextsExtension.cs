namespace Family.Budget.Kernel.Extensions;

using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Domain.Entities.Accounts.Repository;
using Family.Budget.Domain.Entities.Admin.Repository;
using Family.Budget.Domain.Entities.CashFlow.Repository;
using Family.Budget.Domain.Entities.Categories.Repository;
using Family.Budget.Domain.Entities.Currencies.Repository;
using Family.Budget.Domain.Entities.FinancialMovement.Repository;
using Family.Budget.Domain.Entities.PaymentMethods.Repository;
using Family.Budget.Domain.Entities.Registrations.Repository;
using Family.Budget.Domain.Entities.SubCategories.Repository;
using Family.Budget.Infrastructure.Repositories.CashFlows;
using Family.Budget.Infrastructure.Repositories.Categories;
using Family.Budget.Infrastructure.Repositories.Common;
using Family.Budget.Infrastructure.Repositories.Configurations;
using Family.Budget.Infrastructure.Repositories.Context;
using Family.Budget.Infrastructure.Repositories.Currencies;
using Family.Budget.Infrastructure.Repositories.FinancialMovements;
using Family.Budget.Infrastructure.Repositories.PaymentMethod;
using Family.Budget.Infrastructure.Repositories.Registrations;
using Family.Budget.Infrastructure.Repositories.SubCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DbContextsExtension
{
    public static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("PrincipalDatabase");

        if(string.IsNullOrEmpty(conn) is false)
        {
            services.AddDbContext<PrincipalContext>(options =>
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
                options.UseNpgsql(conn!);
            });

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PrincipalContext>();
        db.Database.Migrate();
        
        }
        
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddScoped<IFinancialMovementRepository, FinancialMovementRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRegistrationRepository, RegistrationsRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICashFlowRepository, CashFlowRepository>();

        return services;
    }
}