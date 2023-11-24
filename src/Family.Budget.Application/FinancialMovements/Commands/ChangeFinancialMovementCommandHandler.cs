namespace Family.Budget.Application.FinancialMovements.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.FinancialMovements.ApplicationsErrors;
using Family.Budget.Application.Dto.FinancialMovements.Responses;
using Family.Budget.Application.FinancialMovements.Services;
using Family.Budget.Application.Models;
using Family.Budget.Application.Models.Authorization;
using Family.Budget.Domain.Entities.Accounts.Repository;
using Family.Budget.Domain.Entities.FinancialMovement;
using Family.Budget.Domain.Entities.FinancialMovement.MovementStatuses;
using Family.Budget.Domain.Entities.FinancialMovement.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class ChangeFinancialMovementCommandHandler : BaseCommands, IRequestHandler<ModifyFinancialMovementCommand, FinancialMovementOutput>
{
    private readonly IFinancialMovementRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancialMovementservices _financialMovementServices;
    private readonly ICurrentUserService _userService;
    private readonly IAccountRepository _accountRepository;

    public ChangeFinancialMovementCommandHandler(IFinancialMovementRepository repository,
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

    public async Task<FinancialMovementOutput> Handle(ModifyFinancialMovementCommand command, CancellationToken cancellationToken)
    {
        FinancialMovementOutput ret = null!;

        var entity = await _repository.GetById(command.Id, cancellationToken);

        if (entity == null)
        {
            _notifier.Erros.Add(Errors.FinancialMovementNotFound());
            return null!;
        }

        var account = await _accountRepository.GetByIdandUser(entity.AccountId, _userService.User.UserId, cancellationToken);

        if (account is null)
        {
            _notifier.Erros.Add(Errors.FinancialMovementNotFound());
            return null!;
        }

        var status = MovementStatus.GetByKey<MovementStatus>(command.StatusId);

        if (status is null)
        {
            _notifier.Erros.Add(Dto.MovementStatuses.ApplicationsErrors.Errors.FinancialMovementStatusNotFound());
        }

        var subCategory = account!.SubCategories.FirstOrDefault(x => x.SubCategoryId == command.SubCategoryId);

        if (subCategory is null)
        {
            _notifier.Erros.Add(Dto.SubCategories.Errors.Errors.SubCategoryNotFound());
        }

        var paymentMethod = account!.PaymentMethods.FirstOrDefault(x => x.PaymentMethodId == command.PaymentMethodId);

        if (paymentMethod is null)
        {
            _notifier.Erros.Add(Dto.PaymentMethods.Errors.Errors.PaymentMethodNotFound());
        }

        if (_notifier.Erros.Any())
        {
            return null!;
        }

        if (entity.Date.Month == command.Date.Month && command.Date.Year == entity.Date.Year)
        {
            entity.SetNewValues(command.Date, command.Description, command.Value, subCategory!.SubCategory, entity.Type, status!, paymentMethod!.PaymentMethod);
        }
        else
        {
            entity.SetDeleted();
            var newEntity = FinancialMovement.New(command.Date,
                command.Description,
                command.Value,
                subCategory!.SubCategory,
                entity.Type, status!,
                paymentMethod!.PaymentMethod,
                entity.AccountId);

            await _repository.Insert(entity, cancellationToken);
        }

        await _repository.Update(entity, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return ret;
    }
}
