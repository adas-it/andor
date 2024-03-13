namespace Family.Budget.Application.Models;

using Family.Budget.Application.Dto.Models.Events;
using MediatR;

public record PublishIntegrationsEvents(TopicNames TopicName, IntegrationEvent Data)  : INotification;
