namespace Family.Budget.Application.SubCategories.Services;

using System.Threading.Tasks;
using Family.Budget.Domain.Entities.SubCategories;

public interface ISubCategoryServices
{
    Task Handle(SubCategory entity, CancellationToken cancellationToken);
}
