using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.FinancialMovements.Response;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements.ValueObjects;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.FinancialMovements.Commands;

public record DeleteFinancialMovementCommand : IRequest<ApplicationResult<FinancialMovementOutput>>
{
    public FinancialMovementId FinancialMovementId { get; set; }
}

public class DeleteFinancialMovementCommandHandler(ICommandsFinancialMovementRepository _repository,
        IUnitOfWork _unitOfWork,
        ICommandsAccountRepository _accountRepository) : IRequestHandler<DeleteFinancialMovementCommand, ApplicationResult<FinancialMovementOutput>>
{
    public async Task<ApplicationResult<FinancialMovementOutput>> Handle(DeleteFinancialMovementCommand request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<FinancialMovementOutput>.Success();

        var item = await _repository.GetByIdAsync(request.FinancialMovementId, cancellationToken);

        item.Delete();

        await _repository.UpdateAsync(item, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var output = item.Adapt<FinancialMovementOutput>();

        response.SetData(output);

        return response;
    }
}