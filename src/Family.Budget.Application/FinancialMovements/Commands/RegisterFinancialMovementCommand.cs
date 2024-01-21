namespace Family.Budget.Application.FinancialMovements.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.FinancialMovements.Responses;
using Family.Budget.Application.FinancialMovements.Services;
using Family.Budget.Application.Models;
using Family.Budget.Application.Models.Authorization;
using Family.Budget.Domain.Entities.Accounts.Repository;
using Family.Budget.Domain.Entities.FinancialMovement;
using Family.Budget.Domain.Entities.FinancialMovement.MovementStatuses;
using Family.Budget.Domain.Entities.FinancialMovement.Repository;
using Mapster;
using MediatR;

public record RegisterFinancialMovementCommand : BaseFinancialMovement, IRequest<FinancialMovementOutput>
{
}

public class RegisterFinancialMovementCommandHandler : BaseCommands, IRequestHandler<RegisterFinancialMovementCommand, FinancialMovementOutput>
{
    private readonly IFinancialMovementRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancialMovementservices _financialMovementServices;
    private readonly ICurrentUserService _userService;
    private readonly IAccountRepository _accountRepository;

    public RegisterFinancialMovementCommandHandler(IFinancialMovementRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier,
        IFinancialMovementservices financialMovementServices,
        ICurrentUserService userService,
        IAccountRepository accountRepository) : base(notifier)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _financialMovementServices = financialMovementServices;
        _accountRepository = accountRepository;
        _userService = userService;
    }

    public async Task<FinancialMovementOutput> Handle(RegisterFinancialMovementCommand request, CancellationToken cancellationToken)
    {
        var status = MovementStatus.GetByKey<MovementStatus>(request.StatusId);

        if (status is null)
        {
            _notifier.Warnings.Add(Dto.MovementStatuses.ApplicationsErrors.Errors.FinancialMovementStatusNotFound());
            return null!;
        }

        var account = await _accountRepository.GetByIdandUser(request.AccountId, _userService.User.UserId, cancellationToken);

        if (account is null)
        {
            _notifier.Warnings.Add(Dto.FinancialMovements.ApplicationsErrors.Errors.FinancialMovementNotFound());
            return null!;
        }

        var subCategory = account.SubCategories.FirstOrDefault(x => x.SubCategoryId == request.SubCategoryId);

        if (subCategory is null)
        {
            _notifier.Warnings.Add(Dto.SubCategories.Errors.Errors.SubCategoryNotFound());
            return null!;
        }

        var paymentMethod = account.PaymentMethods.FirstOrDefault(x => x.PaymentMethodId == request.PaymentMethodId);

        if (paymentMethod is null)
        {
            _notifier.Warnings.Add(Dto.PaymentMethods.Errors.Errors.PaymentMethodNotFound());
            return null!;
        }

        var item = FinancialMovement.New(request.Date,
            request.Description,
            request.Value,
            subCategory.SubCategory,
            subCategory.SubCategory.Category.Type,
            status,
            paymentMethod.PaymentMethod,
            account.Id);

        await _financialMovementServices.Handle(item, cancellationToken);

        if (_notifier.Erros.Any())
        {
            return null!;
        }

        await _repository.Insert(item, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return item.Adapt<FinancialMovementOutput>();
    }
}