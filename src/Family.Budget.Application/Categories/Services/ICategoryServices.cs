namespace Family.Budget.Application.Categories.Services;

using System.Threading.Tasks;
using Family.Budget.Domain.Entities.Categories;

public interface ICategoryServices
{
    Task Handle(Category entity, CancellationToken cancellationToken);
}
