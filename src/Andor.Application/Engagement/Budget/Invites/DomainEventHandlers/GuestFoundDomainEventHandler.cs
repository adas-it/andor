using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Communications.IntegrationsEvents.v1;
using Andor.Domain.Administrations.Configurations.Repository;
using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using MediatR;
using System.Reflection;

namespace Andor.Application.Engagement.Budget.Invites.DomainEventHandlers
{
    public sealed record GuestFoundCommand(InviteId InviteId) : IRequest
    {
    }

    internal class GuestFoundDomainEventHandler(ICommandsInviteRepository inviteRepository,
        IUnitOfWork unitOfWork,
        IMessageSenderInterface messageSenderInterface,
        IQueriesConfigurationRepository _configurationRepository)
    : IRequestHandler<GuestFoundCommand>
    {
        public async Task Handle(GuestFoundCommand request, CancellationToken cancellationToken)
        {
            var invite = await inviteRepository.GetByIdAsync(request.InviteId, cancellationToken);

            invite = invite ?? throw new ArgumentNullException(nameof(invite));

            var registrationRule = await _configurationRepository.GetActiveByNameAsync("invite_to_join_account",
            cancellationToken);

            registrationRule = registrationRule ?? throw new InvalidFilterCriteriaException("Configuration not found invite_to_join_account");

            await messageSenderInterface.PubSubSendAsync(new RequestCommunication()
            {
                RuleId = Guid.Parse(registrationRule.Value),
                Email = invite.Email,
                ContentLanguage = "en",
                Values = new Dictionary<string, string>
            {
                { "<name>", invite.Guest.FirstName },
                { "<account_name>", invite.Account.Name }
            }
            }, cancellationToken);


            invite.InvitationMade();

            await unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
