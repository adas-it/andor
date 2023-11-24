namespace Family.Budget.Application.Currencies.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Currencies.Errors;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Currencies.Repository;
using MediatR;

public record RemoveCurrencyCommand(Guid Id) : IRequest;

public class RemoveCurrencyCommandHandler : BaseCommands, IRequestHandler<RemoveCurrencyCommand>
{
    private readonly ICurrencyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveCurrencyCommandHandler(ICurrencyRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier) : base(notifier)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveCurrencyCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetById(request.Id, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(Errors.CurrencyNotFound());
            return;
        }

        await _repository.Delete(item, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return;
    }
}