using Akka.Actor;
using Akka.Hosting;
using Andor.Communications.Application.Actors;
using Andor.Communications.Application.Commands;
using Andor.Communications.Application.Interfaces;
using Andor.Communications.Contracts.Responses;
using Andor.Communications.Domain;
using Andor.Communications.Domain.ValueObjects;
using Andor.Foundation.Application.Commands;
using Andor.Foundation.Contracts.Results;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Communications.Application;

public class RuleCommandsService(ActorRegistry registry) : IRuleCommandsService
{
    private readonly IActorRef _ruleActor = registry.Get<RuleManagerActor>();

    public Task<ApplicationResult<RuleOutput?>> CreateRuleAsync(CreateRuleCommand command)
        => Handler<Rule, RuleOutput?>(command, x => x.ToRuleOutput());

    public Task<ApplicationResult<object?>> SendNotificationAsync(SendNotificationCommand command)
        => Handler<Rule, object?>(command, x => x);

    private async Task<ApplicationResult<TResponse>> Handler<TResult, TResponse>(
        ICommands<RuleId> command, Func<TResult, TResponse> mapper)
        where TResult : class?
        where TResponse : class?
    {
        var response = ApplicationResult<TResponse>.Success();

        var (result, rule) = await _ruleActor.Ask<(DomainResult, TResult)>(command,
            command.CancellationToken);

        if (result.IsFailure)
        {
            await HandleRuleResult.HandleResultRule(result, response);
            return response;
        }

        if (rule != null)
        {
            response.SetData(mapper(rule));
        }

        return response;
    }
}
