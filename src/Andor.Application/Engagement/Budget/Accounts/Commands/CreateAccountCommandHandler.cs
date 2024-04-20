using Andor.Application.Common.Attributes;
using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Account;
using Andor.Domain.Engagement.Budget.Entities.Accounts;
using Andor.Domain.Engagement.Budget.Entities.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Entities.Currencies.ValueObjects;
using Andor.Domain.Engagement.Budget.Entities.PaymentMethods.ValueObjects;
using Andor.Domain.Engagement.Budget.Entities.SubCategories.ValueObjects;
using Andor.Domain.Entities.Admin.Configurations.Repository;
using Andor.Domain.Onboarding.Registrations.Repositories;
using Andor.Domain.Onboarding.Users.ValueObjects;
using FluentValidation;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Commands;

public record CreateAccountCommand : IRequest<ApplicationResult<AccountOutput>>
{
    public CurrencyId CurrencyId { get; set; }
    public UserId UserId { get; set; }
    public string? AccountName { get; set; }
}

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.CurrencyId)
            .NotNull()
            .WithMessage(ValidationConstant.RequiredField);

        RuleFor(x => x.UserId)
            .NotNull()
            .WithMessage(ValidationConstant.RequiredField);
    }
}

public class CreateAccountCommandHandler(IQueriesConfigurationRepository _configurationRepository,
    IQueriesSubCategoryRepository _subCategoryRepository,
    IQueriesPaymentMethodRepository _paymentMethodRepository,
    IQueriesCurrencyRepository _currencyRepository,
    ICommandsAccountRepository _accountRepository,
    IUnitOfWork _unitOfWork)
    : IRequestHandler<CreateAccountCommand, ApplicationResult<AccountOutput>>
{
    [Log]
    [Transaction]
    public async Task<ApplicationResult<AccountOutput>> Handle(CreateAccountCommand request,
        CancellationToken cancellationToken)
    {
        var response = ApplicationResult<AccountOutput>.Success();

        var defaultCategories = await _configurationRepository.GetActiveByNameAsync("subcategories_default", cancellationToken);
        var defaultPaymentMethods = await _configurationRepository.GetActiveByNameAsync("paymentmethods_default", cancellationToken);
        var currency = await _currencyRepository.GetByIdAsync(request.CurrencyId, cancellationToken);

        var subCategoryIds = defaultCategories.Value.Split(',').Select(x => SubCategoryId.Load(x)).ToList();
        var paymentMethodIds = defaultPaymentMethods.Value.Split(',').Select(x => PaymentMethodId.Load(x)).ToList();

        var subCategories = await _subCategoryRepository.GetByIdsAsync(subCategoryIds, cancellationToken);
        var paymentMethod = await _paymentMethodRepository.GetManyByIdsAsync(paymentMethodIds, cancellationToken);

        var categories = subCategories.Select(x => x.Category).Distinct().ToList();

        var (_, entity) = Account.New(request.AccountName, "", categories, subCategories, paymentMethod, new List<Guid>() { request.UserId }, currency);

        await _accountRepository.InsertAsync(entity, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        response.SetData(entity.Adapt<AccountOutput>());

        return response;
    }
}