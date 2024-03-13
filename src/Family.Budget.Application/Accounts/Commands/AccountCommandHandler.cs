namespace Family.Budget.Application.Accounts.Commands;

using Family.Budget.Application;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Accounts.Responses;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Accounts;
using Family.Budget.Domain.Entities.Accounts.Repository;
using Family.Budget.Domain.Entities.Admin.Repository;
using Family.Budget.Domain.Entities.Currencies.Repository;
using Family.Budget.Domain.Entities.PaymentMethods.Repository;
using Family.Budget.Domain.Entities.SubCategories.Repository;
using Mapster;
using MediatR;
public record AccountCommand : IRequest<AccountOutput>
{
    public Guid CurrencyId { get; set; }
    public Guid UserId { get; set; }
    public string AccountName { get; set; }
}

public class AccountCommandHandler : BaseCommands, IRequestHandler<AccountCommand, AccountOutput>
{
    private readonly IConfigurationRepository _configurationRepository;
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AccountCommandHandler(IConfigurationRepository configurationRepository,
        ISubCategoryRepository subCategoryRepository,
        IAccountRepository accountRepository,
        IPaymentMethodRepository paymentMethodRepository,
        ICurrencyRepository currencyRepository,
        IUnitOfWork unitOfWork,
        Notifier notifier) : base(notifier)
    {
        _configurationRepository = configurationRepository;
        _subCategoryRepository = subCategoryRepository;
        _accountRepository = accountRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _currencyRepository = currencyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountOutput> Handle(AccountCommand request, CancellationToken cancellationToken)
    {
        var defaultCategories = await _configurationRepository.GetByNameActive("subcategories_default", cancellationToken);
        var defaultPaymentMethods = await _configurationRepository.GetByNameActive("paymentmethods_default", cancellationToken);
        var currency = await _currencyRepository.GetById(request.CurrencyId, cancellationToken);

        if (defaultCategories == null)
        {
            throw new Exception();
        }

        var subCategories = await _subCategoryRepository.GetByIds(defaultCategories.Value.Split(',').Select(x => new Guid(x)).ToList(), cancellationToken);
        var paymentMethod = await _paymentMethodRepository.GetByIds(defaultPaymentMethods.Value.Split(',').Select(x => new Guid(x)).ToList(), cancellationToken);

        var categories = subCategories.Select(x => x.Category).Distinct().ToList();

        var entity = Account.New(request.AccountName, "", categories, subCategories, paymentMethod, new List<Guid>() { request.UserId }, currency);

        await _accountRepository.Insert(entity, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return entity.Adapt<AccountOutput>();
    }
}