﻿using Andor.Application.Administrations.Configurations.Commands.RegisterConfigurations;
using Andor.Application.Administrations.Configurations.Services;
using Andor.Application.Common.Behaviors;
using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models.Authorizations;
using Andor.Application.Communications.Services.Manager;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Andor.Ioc.Extensions;

public static class ApplicationExtension
{
    public static WebApplicationBuilder AddApplicationExtensionServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(
                Assembly.GetAssembly(typeof(RegisterConfigurationCommandValidator)));

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

        builder.Services.AddScoped<IConfigurationServices, ConfigurationServices>();
        builder.Services.AddScoped<IPartnerManager, PartnerManager>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


        return builder;
    }
}