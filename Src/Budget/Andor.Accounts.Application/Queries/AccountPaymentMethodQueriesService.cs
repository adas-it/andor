using Andor.Accounts.Application.Interfaces;
using Andor.Accounts.Application.Queries.Contracts;
using Andor.Accounts.Contracts.PaymentMethods.Responses;
using Andor.Accounts.Domain.Accounts.ValueObjects;
using Andor.Accounts.Domain.PaymentMethods.ValueObjects;
using Andor.Authorizations.Domain;
using Andor.Foundation.Application.Queries;
using Andor.Foundation.Contracts.Results;

namespace Andor.Accounts.Application.Queries;

public class AccountPaymentMethodQueriesService(IAccountQueriesRepository accountQueriesRepository,
    ICurrentUserService currentUserService) : IAccountPaymentMethodQueriesService
{
    private readonly IAccountQueriesRepository _accountQueriesRepository = accountQueriesRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<ApplicationResult<ListPaymentMethodsOutput?>> GetByAccountIdAndPaymentMethodAsync(ListPaymentMethodsQuery query, CancellationToken cancellationToken)
    {
        var user = _currentUserService.GetCurrentUser();

        var account = await _accountQueriesRepository.GetByIdAsync(query.AccountId, cancellationToken);

        if (account == null || !account.Members.Any(x => x.UserId == user.UserId))
        {
            return ApplicationResult<ListPaymentMethodsOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12226), $"Account with ID '{query.AccountId}' not found or user does not have access.") });
        }

        var accountPaymentMethods = account.PaymentMethods.AsEnumerable();

        if (query.Type.HasValue)
        {
            accountPaymentMethods = accountPaymentMethods.Where(x => x.PaymentMethod.Type.Key == query.Type.Value);
        }

        var querable = accountPaymentMethods.Select(x => x.PaymentMethod.ToPaymentMethodOutput(x.Order))
            .Where(x => x.Name.Contains(query.Search ?? string.Empty, StringComparison.OrdinalIgnoreCase));

        querable = query.Order == SearchOrder.Desc
            ? querable.OrderByDescending(x => x.Name)
            : querable.OrderBy(x => x.Name);

        var paymentMethods = querable.ToList();

        if (paymentMethods.Count == 0)
        {
            return ApplicationResult<ListPaymentMethodsOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12227), $"No payment methods found for account with ID '{query.AccountId}'.") });
        }

        var output = new ListPaymentMethodsOutput(query.Page, query.PerPage, paymentMethods.Count,
            paymentMethods.Skip(query.Page * query.PerPage).Take(query.PerPage).ToList());

        return ApplicationResult<ListPaymentMethodsOutput?>.Success().SetData(output);
    }

    public async Task<ApplicationResult<PaymentMethodOutput?>> GetByAccountIdAndPaymentMethodIdAsync(AccountId accountId, PaymentMethodId paymentMethodId, CancellationToken cancellationToken)
    {
        var user = _currentUserService.GetCurrentUser();

        var account = await _accountQueriesRepository.GetByIdAsync(accountId, cancellationToken);

        if (account == null || !account.Members.Any(x => x.UserId == user.UserId))
        {
            return ApplicationResult<PaymentMethodOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12226), $"Account with ID '{accountId}' not found or user does not have access.") });
        }

        var accountPaymentMethod = account.PaymentMethods.FirstOrDefault(x => x.PaymentMethodId == paymentMethodId);

        if (accountPaymentMethod is null || accountPaymentMethod.PaymentMethod is null)
        {
            return ApplicationResult<PaymentMethodOutput?>.Failure(new List<ErrorModel> { new ErrorModel(ApplicationErrorCode.New(12227), $"Payment method with ID '{paymentMethodId}' not found for account with ID '{accountId}'.") });
        }

        var paymentMethod = accountPaymentMethod.PaymentMethod.ToPaymentMethodOutput(accountPaymentMethod.Order);

        return ApplicationResult<PaymentMethodOutput?>.Success().SetData(paymentMethod);
    }
}
