using MediatR;
using Family.Budget.Application.Dto.Configurations.Requests;
using Family.Budget.Application.Dto.Configurations.Responses;

namespace Family.Budget.Application.Administrations.Commands;

public record ModifyConfigurationCommand : BaseConfiguration, IRequest<ConfigurationOutput>
{
    public Guid Id { get; set; }
    public ModifyConfigurationCommand() : base() { }

    public ModifyConfigurationCommand(Guid id, ModifyConfigurationInput dto)
        : base(dto.Name, dto.Value, dto.Description, dto.StartDate, dto.FinalDate)
    {
        Id = id;
    }

    public ModifyConfigurationCommand(Guid id, string name, string value, string description, DateTimeOffset startDate, DateTimeOffset? finalDate)
        : base(name, value, description, startDate, finalDate)
    {
        Id = id;
    }
}
