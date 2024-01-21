namespace Family.Budget.Application.Administrations.Services;
using System.Linq;
using System.Threading.Tasks;
using Family.Budget.Application.Dto.Configurations.Errors;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Admin;
using Family.Budget.Domain.Entities.Admin.Repository;

public class DateValidationServices : IDateValidationServices
{
    private readonly IConfigurationRepository configurationRepository;
    private readonly Notifier notifier;

    public DateValidationServices(IConfigurationRepository configurationRepository,
        Notifier notifier)
    {
        this.configurationRepository = configurationRepository;
        this.notifier = notifier;
    }

    public async Task Handle(Configuration entity, CancellationToken cancellationToken)
    {
        var listWithSameName = await configurationRepository.GetByName(entity.Name, cancellationToken);

        if (listWithSameName is not null && listWithSameName.Where(x => x.Id != entity.Id).Any())
        {
            if (listWithSameName.Where(x => x.StartDate < entity.StartDate && x.FinalDate > entity.StartDate && x.Id != entity.Id).Any())
            {
                notifier.Erros.Add(ConfigurationErrors.ThereWillCurrentConfigurationStartDate());
            }

            if (listWithSameName.Where(x => x.StartDate < entity.FinalDate && x.FinalDate > entity.FinalDate && x.Id != entity.Id).Any())
            {
                notifier.Erros.Add(ConfigurationErrors.ThereWillCurrentConfigurationEndDate());
            }
        }
    }
}
