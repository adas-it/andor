namespace Family.Budget.Application.Dto.Configurations.IntegrationEvents.v1;

using Family.Budget.Application.Dto.Models.Events;
using System;

public record ConfigurationCreated(Guid Id, 
    string Name, 
    string Value, 
    string Description, 
    DateTimeOffset StartDate, 
    DateTimeOffset? FinalDate)
    : IntegrationEvent(Id,Versions.v1);

public record ConfigurationChanged(Guid Id, 
    string Name, 
    string Value, 
    string Description, 
    DateTimeOffset StartDate, 
    DateTimeOffset? FinalDate)
    : IntegrationEvent(Id, Versions.v1);

public record ConfigurationDeleted(Guid Id)
    : IntegrationEvent(Id, Versions.v1);
