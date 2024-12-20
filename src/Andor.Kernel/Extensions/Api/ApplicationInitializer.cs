﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Andor.Ioc.Extensions.Api;

public static class ApplicationInitializer
{
    private static readonly Assembly DomainAssembly;
    private static readonly Assembly InfrastructureAssembly;
    private static readonly Assembly ApplicationAssembly;

    static ApplicationInitializer()
    {
        DomainAssembly = AppDomain.CurrentDomain.Load("Andor.Domain");
        ApplicationAssembly = AppDomain.CurrentDomain.Load("Andor.Application");
        InfrastructureAssembly = AppDomain.CurrentDomain.Load("Andor.Infrastructure");
    }

    public static WebApplicationBuilder AddApiExtensionServices(this WebApplicationBuilder builder)
    {
        var assembly1 = Assembly.GetExecutingAssembly();

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly1));
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(DomainAssembly));
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(InfrastructureAssembly));
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ApplicationAssembly));

        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.AddHealthChecks();

        return builder;
    }
}
