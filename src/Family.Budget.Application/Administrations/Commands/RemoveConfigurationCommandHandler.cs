namespace Family.Budget.Application.Administrations.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Configurations.Errors;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Admin.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class RemoveConfigurationCommand : IRequest<Unit>
{
    public Guid Id { get; private set; }
    public RemoveConfigurationCommand(Guid Id)
    {
        this.Id = Id;
    }
}

public class RemoveConfigurationCommandHandler : BaseCommands, IRequestHandler<RemoveConfigurationCommand, Unit>
{
    private readonly IConfigurationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveConfigurationCommandHandler(IConfigurationRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier) : base(notifier)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RemoveConfigurationCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetById(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(ConfigurationErrors.ConfigurationNotFound());
            return Unit.Value;
        }

        if (item.StartDate < DateTimeOffset.UtcNow && item.FinalDate < DateTimeOffset.UtcNow)
        {
            var err = ConfigurationErrors.ThisCannotBeDoneOnClosedConfiguration();

            _notifier.Erros.Add(err);

            return Unit.Value;
        }

        if (item.StartDate < DateTimeOffset.UtcNow
            && item.FinalDate > DateTimeOffset.UtcNow)
        {
            item.SetFinalDateToNow();

            var err = ConfigurationErrors.ConfigurationInCourse();

            err.ChangeInnerMessage("The final date was set to now");

            _notifier.Warnings.Add(err);

            await _repository.Update(item, cancellationToken);
        }
        else
        {
            await _repository.Delete(item, cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}