using Andor.Application.Communications.Commands;
using Andor.Application.Dto.Communications.IntegrationsEvents.v1;
using MassTransit;
using MediatR;

namespace Andor.Infrastructure.Communication.Messages.Consumers.Integrations;

public class RequestCommunicationEventConsumer(IMediator _mediator) : IConsumer<RequestCommunication>
{
    public async Task Consume(ConsumeContext<RequestCommunication> context)
    {
        await _mediator.Send(new PublishCommunicationCommand(
            RuleId: context.Message.RuleId,
            Email: context.Message.Email,
            Phone: context.Message.Phone,
            UserId: context.Message.UserId,
            ContentLanguage: context.Message.ContentLanguage,
            Values: context.Message.Values));
    }
}
