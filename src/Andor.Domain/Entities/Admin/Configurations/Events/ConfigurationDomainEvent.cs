namespace Andor.Domain.Entities.Admin.Configurations.Events;

public record ConfigurationCreatedDomainEvent
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Value { get; init; } = "";
    public string Description { get; init; } = "";
    public DateTime StartDate { get; init; }
    public DateTime? ExpireDate { get; init; }
    public string CreatedBy { get; init; } = "";
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }

    public static ConfigurationCreatedDomainEvent FromConfiguration(Configuration Configuration)
        => new ConfigurationCreatedDomainEvent() with
        {
            Id = Configuration.Id,
            Name = Configuration.Name,
            Value = Configuration.Value,
            Description = Configuration.Description,
            StartDate = Configuration.StartDate,
            ExpireDate = Configuration.ExpireDate,
            CreatedBy = Configuration.CreatedBy,
            CreatedAt = Configuration.CreatedAt,
            IsDeleted = Configuration.IsDeleted
        };
}

public record ConfigurationUpdatedDomainEvent
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Value { get; init; } = "";
    public string Description { get; init; } = "";
    public DateTime StartDate { get; init; }
    public DateTime? ExpireDate { get; init; }
    public string CreatedBy { get; init; } = "";
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }

    public static ConfigurationUpdatedDomainEvent FromConfiguration(Configuration Configuration)
        => new ConfigurationUpdatedDomainEvent() with
        {
            Id = Configuration.Id,
            Name = Configuration.Name,
            Value = Configuration.Value,
            Description = Configuration.Description,
            StartDate = Configuration.StartDate,
            ExpireDate = Configuration.ExpireDate,
            CreatedBy = Configuration.CreatedBy,
            CreatedAt = Configuration.CreatedAt,
            IsDeleted = Configuration.IsDeleted
        };
}

public record ConfigurationDeletedDomainEvent
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Value { get; init; } = "";
    public string Description { get; init; } = "";
    public DateTime StartDate { get; init; }
    public DateTime? ExpireDate { get; init; }
    public string CreatedBy { get; init; } = "";
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }

    public static ConfigurationDeletedDomainEvent FromConfiguration(Configuration Configuration)
        => new ConfigurationDeletedDomainEvent() with
        {
            Id = Configuration.Id,
            Name = Configuration.Name,
            Value = Configuration.Value,
            Description = Configuration.Description,
            StartDate = Configuration.StartDate,
            ExpireDate = Configuration.ExpireDate,
            CreatedBy = Configuration.CreatedBy,
            CreatedAt = Configuration.CreatedAt,
            IsDeleted = Configuration.IsDeleted
        };
}
