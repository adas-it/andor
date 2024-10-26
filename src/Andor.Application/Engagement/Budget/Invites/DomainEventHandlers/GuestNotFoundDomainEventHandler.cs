using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Communications.IntegrationsEvents.v1;
using Andor.Domain.Administrations.Configurations.Repository;
using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using MediatR;
using System.Reflection;

namespace Andor.Application.Engagement.Budget.Invites.DomainEventHandlers
{
    public sealed record GuestNotFoundCommand(InviteId InviteId) : IRequest
    {
    }

    internal class GuestNotFoundDomainEventHandler(ICommandsInviteRepository inviteRepository,
        IMessageSenderInterface messageSenderInterface,
        IQueriesConfigurationRepository _configurationRepository)
    : IRequestHandler<GuestNotFoundCommand>
    {
        public async Task Handle(GuestNotFoundCommand request, CancellationToken cancellationToken)
        {
            var invite = await inviteRepository.GetByIdAsync(request.InviteId, cancellationToken);

            invite = invite ?? throw new ArgumentNullException(nameof(invite));

            var registrationRule = await _configurationRepository.GetActiveByNameAsync("invite_to_create_account",
            cancellationToken) ?? throw new InvalidFilterCriteriaException("Configuration not found invite_to_create_account");

            await messageSenderInterface.PubSubSendAsync(new RequestCommunication()
            {
                RuleId = Guid.Parse(registrationRule.Value),
                Email = invite.Email,
                ContentLanguage = "en",
                Values = new Dictionary<string, string>
            {
                { "<guest_name>", invite?.Guest?.FirstName ?? invite?.Email.ToString() ?? ""},
                { "<inviting_name>", invite.Inviting.FirstName }
            }
            }, cancellationToken);
        }
    }
}
