namespace Family.Budget.Application.Currencies.Queries;

using MediatR;
using Family.Budget.Application.Dto.Currencies.Responses;
using Family.Budget.Application.Dto.Models.Request;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Family.Budget.Domain.Entities.Currencies.Repository;
using Family.Budget.Application.Currencies.Adapters;

public record ListCurrenciesQuery
    : PaginatedListInput, IRequest<ListCurrenciesOutput>
{
    public ListCurrenciesQuery(
        int page = 0,
        int perPage = 15,
        string search = "",
        string sort = "",
        Dto.Common.Request.SearchOrder dir = Dto.Common.Request.SearchOrder.Asc
    ) : base(page, perPage, search, sort, dir)
    { }

    public ListCurrenciesQuery()
        : base(0, 15, "", "", Dto.Common.Request.SearchOrder.Asc)
    { }
}

public class ListCurrenciesQueryHandler : IRequestHandler<ListCurrenciesQuery, ListCurrenciesOutput>
{
    private readonly ICurrencyRepository CurrencyRepository;

    public ListCurrenciesQueryHandler(ICurrencyRepository CurrencyRepository)
        => this.CurrencyRepository = CurrencyRepository;

    public async Task<ListCurrenciesOutput> Handle(
        ListCurrenciesQuery request,
        CancellationToken cancellationToken)
    {
        var searchOutput = await CurrencyRepository.Search(
            new(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort,
                (SearchOrder)request.Dir
            ),
            cancellationToken
        );

        return new ListCurrenciesOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(CurrencyAdapter.MapDtoFromDomain)
                .ToList()
        );
    }
}