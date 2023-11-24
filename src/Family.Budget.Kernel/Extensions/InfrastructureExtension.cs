namespace Family.Budget.Kernel.Extensions;

using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Infrastructure;
using Family.Budget.Infrastructure.rabbitmq;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
public static class InfrastructureExtension
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services)
    {
        services.AddSingleton<IMessageSenderInterface, SendMessageRabbitmq>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));

        return services;
    }
}