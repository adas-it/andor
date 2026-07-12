using Akka.Actor;
using Akka.Hosting;
using Andor.Configurations.Application.Actors;
using Andor.Configurations.Application.Commands;
using Andor.Configurations.Application.Interfaces;
using Andor.Configurations.Contracts.Responses;
using Andor.Configurations.Domain;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Configurations.Application;

public class ConfigurationCommandsService(ActorRegistry registry) : IConfigurationCommandsService
{
    private readonly IActorRef _configActor = registry.Get<ConfigurationManagerActor>();

    public Task<ApplicationResult<ConfigurationOutput?>> ChangeConfigurationAsync(ChangeConfigurationCommand command)
        => Handler(command);

    public Task<ApplicationResult<ConfigurationOutput?>> CreateConfigurationAsync(CreateConfigurationCommand command)
        => Handler(command);

    public Task<ApplicationResult<object?>> DeleteConfigurationAsync(DeleteConfigurationCommand command)
        => Handler<Configuration, object?>(command, x => x);

    public Task<ApplicationResult<object?>> DeactivateConfigurationAsync(DeactivateConfigurationCommand command)
        => Handler<Configuration, object?>(command, x => x);

    private Task<ApplicationResult<ConfigurationOutput?>> Handler(ICommands<ConfigurationId> command)
        => Handler<Configuration, ConfigurationOutput?>(command, x => x.ToConfigurationOutput());

    private async Task<ApplicationResult<TResponse>> Handler<TResult, TResponse>(
        ICommands<ConfigurationId> command, Func<TResult, TResponse> mapper)
        where TResult : class?
        where TResponse : class?
    {
        var response = ApplicationResult<TResponse>.Success();

        var (result, config) = await _configActor.Ask<(DomainResult, TResult)>(command,
            command.CancellationToken);

        if (result.IsFailure)
        {
            await HandleConfigurationResult.HandleResultConfiguration(result, response);
            return response;
        }

        if (config != null)
        {
            response.SetData(mapper(config));
        }

        return response;
    }
}
