﻿using Andor.Application.Common.Interfaces;
using Andor.Domain.Administrations.Configurations.Repository;
using Andor.Domain.Administrations.Languages.Repositories;
using Andor.Domain.Communications.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Currencies.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Users.Repositories;
using Andor.Domain.Onboarding.Registrations.Repositories;
using Andor.Infrastructure;
using Andor.Infrastructure.Administrations.Repositories.Configurations;
using Andor.Infrastructure.Administrations.Repositories.Languages;
using Andor.Infrastructure.Communication.Repositories;
using Andor.Infrastructure.Communication.Repositories.Users;
using Andor.Infrastructure.Engagement.Budget.Repositories.Commands;
using Andor.Infrastructure.Engagement.Budget.Repositories.Queries;
using Andor.Infrastructure.Onboarding.Repositories.Registrations;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Context;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Ioc.Extensions.Infrastructures;

public static class DbExtension
{
    public static WebApplicationBuilder AddDbExtension(this WebApplicationBuilder builder)
    {
        var conn = builder.Configuration.GetConnectionString(nameof(PrincipalContext));

        if (string.IsNullOrEmpty(conn) is false)
        {
            builder.Services.AddDbContext<PrincipalContext>(options =>
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();

                options.UseNpgsql(conn, x =>
                {
                    x.EnableRetryOnFailure(5);
                    x.MinBatchSize(1);
                });
            });

            var serviceProvider = builder.Services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PrincipalContext>();
            db.Database.Migrate();
        }

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        builder.Services.AddScoped<ICommandsConfigurationRepository, CommandsConfigurationRepository>();
        builder.Services.AddScoped<IQueriesConfigurationRepository, QueriesConfigurationRepository>();

        builder.Services.AddScoped<ICommandsRegistrationRepository, CommandsRegistrationRepository>();
        builder.Services.AddScoped<IQueriesRegistrationRepository, QueriesRegistrationRepository>();

        builder.Services.AddScoped<ICommandsRecipientRepository, CommandsRecipientRepository>();
        builder.Services.AddScoped<IQueriesRecipientRepository, QueriesRecipientRepository>();

        builder.Services.AddScoped<ICommandsPermissionRepository, CommandsPermissionRepository>();
        builder.Services.AddScoped<IQueriesPermissionRepository, QueriesPermissionRepository>();

        builder.Services.AddScoped<ICommandsRuleRepository, CommandsRuleRepository>();
        builder.Services.AddScoped<IQueriesRuleRepository, QueriesRuleRepository>();

        builder.Services.AddScoped<ICommandsTemplateRepository, CommandsTemplateRepository>();
        builder.Services.AddScoped<IQueriesTemplateRepository, QueriesTemplateRepository>();

        builder.Services.AddScoped<IQueriesRegistrationRepository, QueriesRegistrationRepository>();
        builder.Services.AddScoped<ICommandsRegistrationRepository, CommandsRegistrationRepository>();

        builder.Services.AddScoped<IQueriesSubCategoryRepository, QueriesSubCategoryRepository>();
        builder.Services.AddScoped<ICommandsSubCategoryRepository, CommandsSubCategoryRepository>();

        builder.Services.AddScoped<IQueriesPaymentMethodRepository, QueriesPaymentMethodRepository>();
        builder.Services.AddScoped<ICommandsPaymentMethodRepository, CommandsPaymentMethodRepository>();

        builder.Services.AddScoped<IQueriesCurrencyRepository, QueriesCurrencyRepository>();
        builder.Services.AddScoped<ICommandsCurrencyRepository, CommandsCurrencyRepository>();

        builder.Services.AddScoped<IQueriesAccountRepository, QueriesAccountRepository>();
        builder.Services.AddScoped<ICommandsAccountRepository, CommandsAccountRepository>();

        builder.Services.AddScoped<IQueriesInviteRepository, QueriesInviteRepository>();
        builder.Services.AddScoped<ICommandsInviteRepository, CommandsInviteRepository>();

        builder.Services.AddScoped<IQueriesLanguageRepository, QueriesLanguageRepository>();
        builder.Services.AddScoped<ICommandsLanguageRepository, CommandsLanguageRepository>();

        builder.Services.AddScoped<IQueriesCashFlowRepository, QueriesCashFlowRepository>();
        builder.Services.AddScoped<ICommandsCashFlowRepository, CommandsCashFlowRepository>();

        builder.Services.AddScoped<IQueriesAccountCategoryRepository, QueriesAccountCategoryRepository>();
        //builder.Services.AddScoped<ICommandsCashFlowRepository, CommandsCashFlowRepository>();

        builder.Services.AddScoped<IQueriesFinancialMovementRepository, QueriesFinancialMovementRepository>();
        builder.Services.AddScoped<ICommandsFinancialMovementRepository, CommandsFinancialMovementRepository>();

        builder.Services.AddScoped<IQueriesAccountPaymentMethodRepository, QueriesAccountPaymentMethodRepository>();

        builder.Services.AddScoped<IQueriesAccountSubCategoryRepository, QueriesAccountSubCategoryRepository>();

        builder.Services.AddScoped<ICommandsUserRepository, CommandsUserRepository>();

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        return builder;
    }
}
