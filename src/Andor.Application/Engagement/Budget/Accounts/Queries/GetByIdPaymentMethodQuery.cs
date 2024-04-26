using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.PaymentMethods.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods.ValueObjects;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Queries;

public record GetByIdPaymentMethodQuery(AccountId AccountId,
    PaymentMethodId PaymentMethodId) : IRequest<ApplicationResult<PaymentMethodOutput>>;

public class GetByIdPaymentMethodQueryHandler(IQueriesAccountPaymentMethodRepository _repository)
    : IRequestHandler<GetByIdPaymentMethodQuery, ApplicationResult<PaymentMethodOutput>>
{
    public async Task<ApplicationResult<PaymentMethodOutput>> Handle(GetByIdPaymentMethodQuery request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<PaymentMethodOutput>.Success();

        var item = await _repository.GetByIdAsync(request.AccountId, request.PaymentMethodId, cancellationToken);

        if (item is null)
        {
            return null!;
        }

        response.SetData(item.Adapt<PaymentMethodOutput>());

        return response;
    }
}

