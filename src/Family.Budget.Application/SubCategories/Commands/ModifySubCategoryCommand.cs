namespace Family.Budget.Application.SubCategories.Commands;
using MediatR;
using Family.Budget.Application.Dto.SubCategories.Requests;
using Family.Budget.Application.Dto.SubCategories.Responses;


public record ModifySubCategoryCommand : Dto.SubCategories.Requests.BaseSubCategory, IRequest<SubCategoryOutput>
{
    public Guid Id { get; set; }
    public ModifySubCategoryCommand() : base() { }

    public ModifySubCategoryCommand(Guid id, ModifySubCategoryInput dto)
        : base(dto.Name, dto.Description, dto.StartDate, dto.DeactivationDate, dto.CategoryId)
    {
        Id = id;
    }

    public ModifySubCategoryCommand(Guid id,
        string name,
        string description,
        DateTimeOffset? startDate,
        DateTimeOffset? deactivationDate,
        Guid CategoryId)
        : base(name, description, startDate, deactivationDate, CategoryId)
    {
        Id = id;
    }
}
