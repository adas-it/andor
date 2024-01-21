namespace Family.Budget.Application.Administrations.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Administrations.Services;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Configurations.Errors;
using Family.Budget.Application.Dto.Configurations.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Admin.Repository;
using Mapster;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class ChangeConfigurationCommandHandler : BaseCommands, IRequestHandler<ModifyConfigurationCommand, ConfigurationOutput>
{
    private readonly IConfigurationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateValidationServices _dateValidationServices;

    public ChangeConfigurationCommandHandler(IConfigurationRepository repository,
        IUnitOfWork unitOfWork,
        IDateValidationServices dateValidationServices,
        Notifier notifier) : base(notifier)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _dateValidationServices = dateValidationServices;
    }

    public async Task<ConfigurationOutput> Handle(ModifyConfigurationCommand command, CancellationToken cancellationToken)
    {
        ConfigurationOutput ret = null!;

        var entity = await _repository.GetById(command.Id, cancellationToken);

        if (entity == null)
        {
            _notifier.Erros.Add(ConfigurationErrors.ConfigurationNotFound());
            return null!;
        }

        if (entity.StartDate < DateTimeOffset.UtcNow
            && entity.FinalDate < DateTimeOffset.UtcNow
            && (entity.StartDate != command.StartDate
                || entity.FinalDate != command.FinalDate
                || entity.Name != command.Name
                || entity.Value != command.Value))
        {
            _notifier.Erros.Add(ConfigurationErrors.OnlyDescriptionAreAvaliableToChangedOnClosedConfiguration());
            return null!;
        }

        if (entity.StartDate < DateTimeOffset.UtcNow
            && entity.FinalDate > DateTimeOffset.UtcNow
            && entity.Name != command.Name)
        {
            _notifier.Erros.Add(ConfigurationErrors.ItsNotAllowedToChangeName());
            return null!;
        }

        if (entity.StartDate < DateTimeOffset.UtcNow
            && entity.FinalDate > DateTimeOffset.UtcNow
            && entity.StartDate != command.StartDate)
        {
            _notifier.Erros.Add(ConfigurationErrors.ItsNotAllowedToChangeInitialDate());
            return null!;
        }

        if (entity.StartDate < DateTimeOffset.UtcNow
            && entity.FinalDate > DateTimeOffset.UtcNow
            && entity.FinalDate != command.FinalDate
            && command.FinalDate < DateTimeOffset.UtcNow)
        {
            _notifier.Warnings.Add(ConfigurationErrors.ItsNotAllowedToChangeFinalDatetoBeforeToday());
            command.FinalDate = DateTimeOffset.UtcNow;
        }

        if (entity.StartDate < DateTimeOffset.UtcNow
            && entity.FinalDate > DateTimeOffset.UtcNow
            && entity.Value != command.Value)
        {
            entity.ChangeConfiguration(entity.Name,
            entity.Value,
            entity.Description,
            entity.StartDate,
            DateTimeOffset.UtcNow);

            await _repository.Update(entity, cancellationToken);

            //var newOne = Configuration.New(command.Name, command.Value, command.Description, DateTimeOffset.UtcNow, command.FinalDate);

            //await _repository.Insert(newOne, cancellationToken);
        }
        else
        {
            entity.ChangeConfiguration(command.Name,
                command.Value,
                command.Description,
                command.StartDate,
                command.FinalDate);

            await _dateValidationServices.Handle(entity, cancellationToken);

            await _repository.Update(entity, cancellationToken);

            ret = entity.Adapt<ConfigurationOutput>();
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return ret;
    }
}
