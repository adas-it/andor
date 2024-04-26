using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.PaymentMethods.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Queries;

public record ListPaymentMethodsQuery
    : PaginatedListInput, IRequest<ApplicationResult<ListPaymentMethodsOutput>>
{
    public AccountId AccountId { get; set; }
    public int Type { get; set; }
}

public class ListPaymentMethodsQueryHandler(IQueriesAccountPaymentMethodRepository repository)
    : IRequestHandler<ListPaymentMethodsQuery, ApplicationResult<ListPaymentMethodsOutput>>
{
    public async Task<ApplicationResult<ListPaymentMethodsOutput>> Handle(ListPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ListPaymentMethodsOutput>.Success();

        var movementType = request.Type == MovementType.MoneyDeposit.Key ? MovementType.MoneyDeposit : MovementType.MoneySpending;
        var searchOutput = await repository.SearchAsync(
            new(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort,
                (Andor.Domain.SeedWork.Repositories.ResearchableRepository.SearchOrder)request.Dir,
                movementType,
                request.AccountId
            ),
            cancellationToken
        );

        var output = new ListPaymentMethodsOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items.Select(x => x.Adapt<PaymentMethodOutput>()).ToList()
        );

        response.SetData(output);

        return response;
    }
}