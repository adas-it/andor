using Andor.Application.Common;
using Andor.Application.Communications.Interfaces;
using Andor.Application.Communications.Services.Manager;
using Andor.Application.Communications.Services.PartnerHandler;
using Andor.Communications.Domain.Repositories;
using Andor.Communications.Infrastructure;
using Andor.Communications.Infrastructure.Context;
using Andor.Infrastructure.Communication.Gateway;
using Andor.ServiceDefaults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<CommunicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Communication")));

builder.Services.AddOptions<ApplicationSettings>()
    .Bind(builder.Configuration.GetSection("ApplicationSettings"));

builder.Services.AddScoped<ISMTP, Smtp>();
builder.Services.AddScoped<ICommandsRuleRepository, CommandsRuleRepository>();
builder.Services.AddScoped<IPartner, InHousePartner>();
builder.Services.AddScoped<IPartnerManager, PartnerManager>();

builder.Build().Run();
