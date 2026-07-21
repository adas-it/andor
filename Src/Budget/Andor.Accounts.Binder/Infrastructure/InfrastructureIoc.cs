using Andor.Accounts.Binder.Outbox;
using Andor.Accounts.Domain.Accounts.Repositories;
using Andor.Accounts.Domain.CashFlows.Repositories;
using Andor.Accounts.Domain.Categories.Repositories;
using Andor.Accounts.Domain.Currencies.Repositories;
using Andor.Accounts.Domain.FinancialMovements.Repositories;
using Andor.Accounts.Domain.PaymentMethods.Repositories;
using Andor.Accounts.Domain.SubCategories.Repositories;
using Andor.Accounts.Infrastructure;
using Andor.Foundation.Infrastructure.Messaging;
using Andor.Foundation.Infrastructure.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Accounts.Binder.Infrastructure;

internal static class InfrastructureIoc
{
    internal static IServiceCollection WithAccountsInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.WithAccountsDbContext(configuration);

        services.WithAzureServiceBusMessaging(configuration);

        services.AddScoped<ICommandsAccountRepository, CommandsAccountRepository>();

        services.AddScoped<ICommandsCurrencyRepository, CommandsCurrencyRepository>();

        services.AddScoped<ICommandsCategoryRepository, CommandsCategoryRepository>();

        services.AddScoped<ICommandsSubCategoryRepository, CommandsSubCategoryRepository>();

        services.AddScoped<ICommandsPaymentMethodRepository, CommandsPaymentMethodRepository>();

        services.AddScoped<ICommandsFinancialMovementRepository, CommandsFinancialMovementRepository>();

        services.AddScoped<ICommandsCashFlowRepository, CommandsCashFlowRepository>();

        services.AddScoped<ICashFlowAppliedMovementRepository, CashFlowAppliedMovementRepository>();

        services.AddScoped<IOutboxContextProvider, AccountsOutboxContextProvider>();

        services.AddHostedService<OutboxDispatcher>();

        return services;
    }
}
