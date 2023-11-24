namespace Family.Budget.Application.SubCategories.Services;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.SubCategories;
using Family.Budget.Domain.Entities.SubCategories.Repository;
using System.Linq;
using System.Threading.Tasks;

public class SubCategoryServices : ISubCategoryServices
{
    private readonly ISubCategoryRepository SubCategoryRepository;
    private readonly Notifier notifier;

    public SubCategoryServices(ISubCategoryRepository SubCategoryRepository,
        Notifier notifier)
    {
        this.SubCategoryRepository = SubCategoryRepository;
        this.notifier = notifier;
    }

    public async Task Handle(SubCategory entity, CancellationToken cancellationToken)
    {
        var listWithSameName = await SubCategoryRepository.GetByName(entity.Name, cancellationToken);

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
