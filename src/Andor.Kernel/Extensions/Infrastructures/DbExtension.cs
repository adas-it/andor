using Andor.Application.Common.Interfaces;
using Andor.Domain.Entities.Admin.Configurations.Repository;
using Andor.Domain.Entities.Onboarding.Registrations.Repositories;
using Andor.Infrastructure;
using Andor.Infrastructure.Repositories.Common;
using Andor.Infrastructure.Repositories.Configurations;
using Andor.Infrastructure.Repositories.Context;
using Andor.Infrastructure.Repositories.Onboarding.Registrations;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Kernel.Extensions.Infrastructures;

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

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        return builder;
    }
}
