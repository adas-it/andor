using Akka.Actor;
using Andor.Application.Communications.Services.Manager;
using Andor.Authorizations.Application;
using Andor.Authorizations.Domain;
using Andor.Communications.Application.Commands;
using Andor.Communications.Domain;
using Andor.Communications.Domain.Errors;
using Andor.Communications.Domain.Repositories;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Domain.ValuesObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Communications.Application.Actors;

public class RuleActor : ReceiveActor, IWithUnboundedStash
{
    private readonly RuleId _id;
    private Rule? _rule;
    private readonly IServiceProvider _serviceProvider;

    public IStash? Stash { get; set; }

    public RuleActor(RuleId id, IServiceProvider serviceProvider)
    {
        _id = id;
        _serviceProvider = serviceProvider;

        Become(Loading);
    }

    protected override void PreStart()
    {
        Self.Tell(new PreLoadRule(_id));
        base.PreStart();
    }

    private void Loading()
    {
        ReceiveAsync<PreLoadRule>(async _ =>
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsRuleRepository>();

            var result = await repo.GetByIdAsync(_id, CancellationToken.None);

            if (result == null)
                return;

            _rule = result;

            Become(Ready);
            Stash!.UnstashAll();
        });

        ReceiveAsync<CreateRuleCommand>(HandleCreateAsync);

        ReceiveAny(_ => Stash!.Stash());
    }

    private async Task HandleCreateAsync(CreateRuleCommand cmd)
    {
        using var scope = _serviceProvider.CreateScope();

        var userContext = scope.ServiceProvider.GetRequiredService<IUserContextAccessor>();
        userContext.CurrentUser = cmd.CurrentUser;

        var validator = scope.ServiceProvider.GetRequiredService<IRuleValidator>();
        var repo = scope.ServiceProvider.GetRequiredService<ICommandsRuleRepository>();
        var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();

        var user = currentUser.GetCurrentUser();

        var (domainResult, rule) = await Rule.NewAsync(
            _id,
            cmd.Name,
            cmd.Type,
            [],
            cmd.Force,
            user.UserId,
            validator,
            cmd.CancellationToken);

        if (domainResult.IsSuccess && rule != null)
        {
            foreach (var t in cmd.Templates)
            {
                var (templateResult, template) = Template.New(t.Value, t.ContentLanguage, t.Title, t.Partner, rule);

                if (templateResult.IsFailure)
                {
                    Sender.Tell((templateResult, (Rule?)null));
                    userContext.CurrentUser = null;
                    return;
                }

                rule.Templates.Add(template!);
            }
        }

        if (rule?.Events.Count > 0)
            await repo.PersistAsync(rule, cmd.CancellationToken);

        _rule = rule;
        Sender.Tell((domainResult, rule));

        if (rule != null)
        {
            Become(Ready);
            Stash!.UnstashAll();
        }

        userContext.CurrentUser = null;
    }

    private void Ready()
    {
        ReceiveAsync<SendNotificationCommand>(HandleSendNotificationAsync);
    }

    private async Task HandleSendNotificationAsync(SendNotificationCommand cmd)
    {
        if (_rule is null)
        {
            DomainResult notFound = DomainResult.Failure(
                errors: new[] {
                    new Notification(nameof(_id), $"Rule with id '{_id}' not found",
                        CommunicationsErrorCodes.RuleNotFound)
                });

            Sender.Tell((notFound, (Rule?)null));
            return;
        }

        using var scope = _serviceProvider.CreateScope();

        var userContext = scope.ServiceProvider.GetRequiredService<IUserContextAccessor>();
        userContext.CurrentUser = cmd.CurrentUser;

        var partnerManager = scope.ServiceProvider.GetRequiredService<IPartnerManager>();

        var template = _rule.Templates.FirstOrDefault(x => x.Title == cmd.TemplateTitle);

        if (template is null)
        {
            DomainResult notFound = DomainResult.Failure(
                errors: new[] {
                    new Notification(nameof(cmd.TemplateTitle),
                        $"Template with title '{cmd.TemplateTitle}' not found in rule '{_id}'",
                        CommunicationsErrorCodes.RuleNotFound)
                });

            Sender.Tell((notFound, (Rule?)null));
            userContext.CurrentUser = null;
            return;
        }

        var partner = partnerManager.GetPartnerHandler(template.Partner);

        await partner.SendEmail(cmd.RecipientEmail, cmd.Subject, template, cmd.Values, cmd.CancellationToken);

        Sender.Tell((DomainResult.Success(), _rule));

        userContext.CurrentUser = null;
    }

    private record PreLoadRule(RuleId Id);
}
