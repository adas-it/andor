using Akka.Actor;
using Andor.Authorizations.Application;
using Andor.Authorizations.Domain;
using Andor.Configurations.Application.Commands;
using Andor.Configurations.Domain;
using Andor.Configurations.Domain.Errors;
using Andor.Configurations.Domain.Repositories;
using Andor.Configurations.Domain.ValueObjects;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Domain.ValuesObjects;
using Microsoft.Extensions.DependencyInjection;

public class ConfigurationActor : ReceiveActor, IWithUnboundedStash
{
    private readonly ConfigurationId _id;
    private Configuration? _configuration;
    private readonly IServiceProvider _serviceProvider;

    public IStash? Stash { get; set; }

    public ConfigurationActor(ConfigurationId id, IServiceProvider serviceProvider)
    {
        _id = id;
        _serviceProvider = serviceProvider;

        Become(Loading);
    }

    protected override void PreStart()
    {
        Self.Tell(new PreLoadConfiguration(_id));
        base.PreStart();
    }

    private void Loading()
    {
        ReceiveAsync<PreLoadConfiguration>(async _ =>
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsConfigurationRepository>();

            var result = await repo.GetByIdAsync(_id, CancellationToken.None);

            if (result == null)
                return;

            _configuration = result;

            Become(Ready);
            Stash!.UnstashAll();
        });

        ReceiveAsync<CreateConfigurationCommand>(HandleCreateAsync);

        ReceiveAny(_ => Stash!.Stash());
    }

    private async Task HandleCreateAsync(CreateConfigurationCommand cmd)
    {
        using var scope = _serviceProvider.CreateScope();

        var userContext = scope.ServiceProvider.GetRequiredService<IUserContextAccessor>();
        userContext.CurrentUser = cmd.CurrentUser;

        var (validator, repo, currentUser, authorizationDomain) = GetDependencies(scope);

        var hasPermissionToCreate = await authorizationDomain.IsAuthorizedAsync(
            ConfigurationPermissions.CreateConfiguration, null);

        if (!hasPermissionToCreate)
        {
            var result = DomainResult.Failure(new List<Notification>()
            {
                new Notification("", "You do not have permission to do this action.",
                    ConfigurationsErrorCodes.ActionNotAllowed)
            });
            Sender.Tell((result, (Configuration?)null));
            userContext.CurrentUser = null;
            return;
        }

        if (cmd.Force)
        {
            var hasPermissionToSkipValidation = await authorizationDomain.IsAuthorizedAsync(
                ConfigurationPermissions.SkipConfigurationValidations, null);

            if (!hasPermissionToSkipValidation)
            {
                var result = DomainResult.Failure(new List<Notification>()
                {
                    new Notification("", "You do not have permission to do this action.",
                        ConfigurationsErrorCodes.ActionNotAllowed)
                });
                Sender.Tell((result, (Configuration?)null));
                userContext.CurrentUser = null;
                return;
            }
        }

        var user = currentUser.GetCurrentUser();

        var (domainResult, config) = await Configuration.NewAsync(
            _id,
            cmd.Name,
            cmd.Value,
            cmd.Description,
            cmd.StartDate,
            cmd.ExpireDate,
            ConfigurationType.Generic,
            cmd.Force,
            user.UserId,
            validator,
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

        userContext.CurrentUser = null;
    }

    private void Ready()
    {
        ReceiveAsync<ChangeConfigurationCommand>(cmd =>
            HandleUpdateAsync(cmd, (v, c, userId, ct) =>
                c.UpdateAsync(cmd.Value, cmd.Description, cmd.StartDate, cmd.ExpireDate, userId, v, ct)));

        ReceiveAsync<DeactivateConfigurationCommand>(cmd =>
            HandleUpdateAsync(cmd, (v, c, userId, ct) =>
                c.DeactivateAsync(userId, v, ct)));

        ReceiveAsync<DeleteConfigurationCommand>(cmd =>
            HandleUpdateAsync(cmd, (v, c, userId, ct) =>
                c.DeleteAsync(userId, ct)));
    }

    private async Task HandleUpdateAsync<TCommand>(
        TCommand cmd,
        Func<IConfigurationValidator, Configuration, Guid, CancellationToken, Task<DomainResult>> operation)
        where TCommand : ICommands<ConfigurationId>
    {
        if (_configuration is null)
        {
            DomainResult notFound = DomainResult.Failure(
                errors: new[] {
                    new Notification(nameof(_id), $"Configuration with id '{_id}' not found",
                        ConfigurationsErrorCodes.ConfigurationNotFound)
                });

            Sender.Tell((notFound, (Configuration?)null));
            return;
        }

        using var scope = _serviceProvider.CreateScope();

        var userContext = scope.ServiceProvider.GetRequiredService<IUserContextAccessor>();
        userContext.CurrentUser = cmd.CurrentUser;

        var (validator, repo, currentUser, authorizationDomain) = GetDependencies(scope);

        var user = currentUser.GetCurrentUser();

        var result = await operation(validator, _configuration, user.UserId, cmd.CancellationToken);

        if (_configuration.Events.Count > 0)
            await repo.PersistAsync(_configuration, cmd.CancellationToken);

        Sender.Tell((result, _configuration));

        userContext.CurrentUser = null;
    }

    private (IConfigurationValidator, ICommandsConfigurationRepository, ICurrentUserService, AuthorizationDomain)
        GetDependencies(IServiceScope scope)
        => (scope.ServiceProvider.GetRequiredService<IConfigurationValidator>(),
            scope.ServiceProvider.GetRequiredService<ICommandsConfigurationRepository>(),
            scope.ServiceProvider.GetRequiredService<ICurrentUserService>(),
            scope.ServiceProvider.GetRequiredService<AuthorizationDomain>());

    private record PreLoadConfiguration(ConfigurationId Id);
}
