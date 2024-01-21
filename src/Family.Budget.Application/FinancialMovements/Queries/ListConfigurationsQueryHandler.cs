namespace Family.Budget.Application.FinancialMovements.Queries;

using Family.Budget.Application.Dto.FinancialMovements.Responses;
using Family.Budget.Application.Dto.Models.Request;
using Family.Budget.Domain.Entities.FinancialMovement;
using Family.Budget.Domain.Entities.FinancialMovement.Repository;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Mapster;
using MediatR;

public record ListFinancialMovementsQuery
    : PaginatedListInput, IRequest<ListFinancialMovementsOutput>
{
    public ListFinancialMovementsQuery(
        int page = 0,
        int perPage = 15,
        string search = "",
        string sort = "",
        Dto.Common.Request.SearchOrder dir = Dto.Common.Request.SearchOrder.Asc
    ) : base(page, perPage, search, sort, dir)
    { }

    public ListFinancialMovementsQuery()
        : base(0, 15, "", "", Dto.Common.Request.SearchOrder.Asc)
    { }

    public int? Year { get; set; }
    public int? Month { get; set; }
    public Guid AccountId { get; set; }
}

public class ListFinancialMovementsQueryHandler : IRequestHandler<ListFinancialMovementsQuery, ListFinancialMovementsOutput>
{
    private readonly IFinancialMovementRepository _repository;

    public ListFinancialMovementsQueryHandler(IFinancialMovementRepository repository)
        => _repository = repository;

    public async Task<ListFinancialMovementsOutput> Handle(
        ListFinancialMovementsQuery request,
        CancellationToken cancellationToken)
    {
        var searchOutput = await _repository.Search(
            new(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort ?? nameof(FinancialMovement.Date),
                (SearchOrder)request.Dir,
                request.AccountId,
                request.Year ?? DateTime.Today.Year,
                request.Month ?? DateTime.Today.Month
            ),
            cancellationToken
        );

        return new ListFinancialMovementsOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(x => x.Adapt<FinancialMovementOutput>())
                .ToList()
        );
    }
}