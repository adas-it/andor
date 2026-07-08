using Akka.Actor;
using Akka.Hosting;
using Andor.Assets.Application.Actors;
using Andor.Assets.Application.Commands;
using Andor.Assets.Application.Interfaces;
using Andor.Assets.Contracts;
using Andor.Assets.Domain.Investments.Areas;
using Andor.Assets.Domain.Investments.Areas.ValueObjects;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Assets.Application;

public class AreaCommandsService(ActorRegistry registry) : IAreaCommandsService
{
    private readonly IActorRef _configActor = registry.Get<AreaManagerActor>();

    public Task<ApplicationResult<AreaOutput?>> CreateAreaAsync(CreateAreaCommand command)
        => Handler(command);

    private Task<ApplicationResult<AreaOutput?>> Handler(ICommands<AreaId> command)
        => Handler<Area, AreaOutput?>(command, x => x.ToAreaOutput());


    private async Task<ApplicationResult<TResponse>> Handler<TResult, TResponse>(
        ICommands<AreaId> command, Func<TResult, TResponse> mapper)
        where TResult : class?
        where TResponse : class?
    {
        var response = ApplicationResult<TResponse>.Success();

        var (result, config) = await _configActor.Ask<(DomainResult, TResult)>(command,
            command.CancellationToken);

        if (result.IsFailure)
        {
            await HandleAreaResult.HandleResultConfiguration(result, response);
            return response;
        }

        if (config != null)
        {
            response.SetData(mapper(config));
        }

        return response;
    }
}
