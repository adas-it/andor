namespace Family.Budget.Domain.Entities.Admin.DomainEvents;

using Family.Budget.Domain.SeedWork;

public record ConfigurationCreatedDomainEvent : DomainEventBase
{
	public ConfigurationCreatedDomainEvent(Configuration configuration) : base(nameof(ConfigurationCreatedDomainEvent))
	{
        Configuration = configuration;
    }

    public Configuration Configuration { get; set; }
}
