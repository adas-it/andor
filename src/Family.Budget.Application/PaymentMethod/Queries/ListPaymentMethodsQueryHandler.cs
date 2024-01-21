namespace Family.Budget.Application.PaymentMethod.Queries;

using MediatR;
using Family.Budget.Application.Dto.PaymentMethods.Responses;
using Family.Budget.Application.Dto.Models.Request;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Family.Budget.Domain.Entities.PaymentMethods.Repository;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Application.PaymentMethod.Adapters;

public record ListPaymentMethodsQuery
    : PaginatedListInput, IRequest<ListPaymentMethodsOutput>
{
    public ListPaymentMethodsQuery(
        int page = 0,
        int perPage = 15,
        string search = "",
        string sort = "",
        Dto.Common.Request.SearchOrder dir = Dto.Common.Request.SearchOrder.Asc
    ) : base(page, perPage, search, sort, dir)
    { }

    public ListPaymentMethodsQuery()
        : base(0, 15, "", "", Dto.Common.Request.SearchOrder.Asc)
    { }

    public string Type { get; set; }
}

public class ListPaymentMethodsQueryHandler : IRequestHandler<ListPaymentMethodsQuery, ListPaymentMethodsOutput>
{
    private readonly IPaymentMethodRepository PaymentMethodRepository;

    public ListPaymentMethodsQueryHandler(IPaymentMethodRepository PaymentMethodRepository)
        => this.PaymentMethodRepository = PaymentMethodRepository;

    public async Task<ListPaymentMethodsOutput> Handle(
        ListPaymentMethodsQuery request,
        CancellationToken cancellationToken)
    {
        var movementType = request.Type == MovementType.MoneyDeposit.Key.ToString() ? MovementType.MoneyDeposit : MovementType.MoneySpending;

        var searchOutput = await PaymentMethodRepository.Search(
            new(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort,
                (SearchOrder)request.Dir
            ),
            movementType,
            cancellationToken
        );

        return new ListPaymentMethodsOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(PaymentMethodAdapter.MapDtoFromDomain)
                .ToList()
        );
    }
}