namespace Family.Budget.Application.Categories.Commands;
using MediatR;
using Family.Budget.Application.Dto.Categories.Requests;
using Family.Budget.Application.Dto.Categories.Responses;


public record ModifyCategoryCommand : Dto.Categories.Requests.BaseCategory, IRequest<CategoryOutput>
{
    public Guid Id { get; set; }
    public ModifyCategoryCommand() : base() { }

    public ModifyCategoryCommand(Guid id, ModifyCategoryInput dto)
        : base(dto.Name, dto.Description, dto.StartDate, dto.DeactivationDate, dto.MovementTypeId)
    {
        Id = id;
    }

    public ModifyCategoryCommand(Guid id, string name, string description,
        DateTimeOffset? startDate, DateTimeOffset? deactivationDate, int movementTypeId)
        : base(name, description, startDate, deactivationDate, movementTypeId)
    {
        Id = id;
    }
}
