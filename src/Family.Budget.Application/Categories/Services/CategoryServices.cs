namespace Family.Budget.Application.Categories.Services;

using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Categories;
using Family.Budget.Domain.Entities.Categories.Repository;
using System.Linq;
using System.Threading.Tasks;

public class CategoryServices : ICategoryServices
{
    private readonly ICategoryRepository categoryRepository;
    private readonly Notifier notifier;

    public CategoryServices(ICategoryRepository categoryRepository,
        Notifier notifier)
    {
        this.categoryRepository = categoryRepository;
        this.notifier = notifier;
    }

    public async Task Handle(Category entity, CancellationToken cancellationToken)
    {
        var listWithSameName = await categoryRepository.GetByName(entity.Name, cancellationToken);

        if (listWithSameName is not null && listWithSameName.Where(x => x.Id != entity.Id).Any())
        {
            if (listWithSameName.Where(x => x.StartDate < entity.StartDate && x.DeactivationDate > entity.StartDate && x.Id != entity.Id).Any())
            {
                //notifier.Erros.Add(Errors.ThereWillCurrentConfigurationStartDate());
            }

            if (listWithSameName.Where(x => x.StartDate < entity.DeactivationDate && x.DeactivationDate > entity.DeactivationDate && x.Id != entity.Id).Any())
            {
                //notifier.Erros.Add(Errors.ThereWillCurrentConfigurationEndDate());
            }
        }
    }
}
