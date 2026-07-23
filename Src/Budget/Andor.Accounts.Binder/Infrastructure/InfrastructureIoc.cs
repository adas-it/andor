using Andor.Accounts.Application.Interfaces;
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
        _ = services.WithAccountsDbContext(configuration);

        _ = services.WithAzureServiceBusMessaging(configuration);

        _ = services.AddScoped<ICommandsAccountRepository, CommandsAccountRepository>();

        _ = services.AddScoped<ICommandsCurrencyRepository, CommandsCurrencyRepository>();

        _ = services.AddScoped<ICommandsCategoryRepository, CommandsCategoryRepository>();

        _ = services.AddScoped<ICommandsSubCategoryRepository, CommandsSubCategoryRepository>();

        _ = services.AddScoped<ICommandsPaymentMethodRepository, CommandsPaymentMethodRepository>();

        _ = services.AddScoped<ICommandsFinancialMovementRepository, CommandsFinancialMovementRepository>();

        _ = services.AddScoped<ICommandsCashFlowRepository, CommandsCashFlowRepository>();

        _ = services.AddScoped<ICashFlowAppliedMovementRepository, CashFlowAppliedMovementRepository>();

        _ = services.AddScoped<IOutboxContextProvider, AccountsOutboxContextProvider>();

        _ = services.AddHostedService<OutboxDispatcher>();

        _ = services.AddScoped<IAccountQueriesRepository, AccountQueriesRepository>();

        return services;
    }
}
