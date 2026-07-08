using Akka.Actor;
using Andor.Assets.Application.Commands;
using Andor.Assets.Domain.Investments.Areas;
using Andor.Assets.Domain.Investments.Areas.Repositories;
using Andor.Assets.Domain.Investments.Areas.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Assets.Application.Actors;

public class AreaActor : ReceiveActor, IWithUnboundedStash
{

    private readonly AreaId _id;
    private Area? _configuration;
    private readonly IServiceProvider _serviceProvider;

    public IStash? Stash { get; set; }

    public AreaActor(AreaId id, IServiceProvider serviceProvider)
    {
        _id = id;
        _serviceProvider = serviceProvider;

        Become(Loading);
    }

    protected override void PreStart()
    {
        Self.Tell(new PreLoadArea(_id));
        base.PreStart();
    }

    private void Loading()
    {
        ReceiveAsync<PreLoadArea>(async _ =>
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsAreaRepository>();

            var result = await repo.GetByIdAsync(_id, CancellationToken.None);

            if (result == null)
                return;

            _configuration = result;

            Become(Ready);
            Stash!.UnstashAll();
        });

        ReceiveAsync<CreateAreaCommand>(HandleCreateAsync);

        ReceiveAny(_ => Stash!.Stash());
    }

    private void Ready()
    {
        ReceiveAsync<AddTickerCommand>(HandleUpdateAsync);
    }


    private Task HandleUpdateAsync(AddTickerCommand cmd)
    {
        return Task.CompletedTask;
    }

    private async Task HandleCreateAsync(CreateAreaCommand cmd)
    {
        using var scope = _serviceProvider.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<ICommandsAreaRepository>();

        var (domainResult, config) = await Area.NewAsync(
            _id,
            cmd.Name,
            cmd.CurrentUser.UserId,
            cmd.CancellationToken);

        if (config?.Events.Count > 0)
            await repo.PersistAsync(config, cmd.CancellationToken);

        _configuration = config;
        Sender.Tell((domainResult, config));

        if (config != null)
        {
            Become(Ready);
            Stash!.UnstashAll();
        }
    }


    private record PreLoadArea(AreaId Id);

}
