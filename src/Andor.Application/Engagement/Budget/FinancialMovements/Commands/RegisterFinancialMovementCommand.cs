using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.FinancialMovements.Response;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Currencies.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.FinancialMovements;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementStatuses;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.FinancialMovements.Commands;

public record RegisterFinancialMovementCommand : IRequest<ApplicationResult<FinancialMovementOutput>>
{
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }
    public SubCategoryId SubCategoryId { get; set; }
    public int StatusId { get; set; }
    public CurrencyId CurrencyId { get; set; }
    public PaymentMethodId PaymentMethodId { get; set; }
    public AccountId AccountId { get; set; }
}

public class RegisterFinancialMovementCommandHandler(ICommandsFinancialMovementRepository _repository,
        IUnitOfWork _unitOfWork,
        ICommandsAccountRepository _accountRepository) : IRequestHandler<RegisterFinancialMovementCommand, ApplicationResult<FinancialMovementOutput>>
{
    public async Task<ApplicationResult<FinancialMovementOutput>> Handle(RegisterFinancialMovementCommand request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<FinancialMovementOutput>.Success();

        var status = MovementStatus.GetByKey<MovementStatus>(request.StatusId);

        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        var subCategory = account.SubCategories.FirstOrDefault(x => x.SubCategoryId == request.SubCategoryId);

        var paymentMethod = account.PaymentMethods.FirstOrDefault(x => x.PaymentMethodId == request.PaymentMethodId);

        var (_, item) = FinancialMovement.New(request.Date,
            request.Description,
            subCategory.SubCategory,
            subCategory.SubCategory.Category.Type,
            status,
            paymentMethod.PaymentMethod,
            account,
            request.Value);

        await _repository.InsertAsync(item, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var output = item.Adapt<FinancialMovementOutput>();

        response.SetData(output);

        return response;
    }
}