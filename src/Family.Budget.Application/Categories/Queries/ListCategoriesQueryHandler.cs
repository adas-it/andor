namespace Family.Budget.Application.Categories.Queries;

using Family.Budget.Application.Dto.Categories.Responses;
using Family.Budget.Application.Dto.Models.Request;
using Family.Budget.Domain.Entities.Categories.Repository;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Mapster;
using MediatR;

public record ListCategoriesQuery
    : PaginatedListInput, IRequest<ListCategoriesOutput>
{
    public ListCategoriesQuery(
        int page = 0,
        int perPage = 15,
        string search = "",
        string sort = "",
        Dto.Common.Request.SearchOrder dir = Dto.Common.Request.SearchOrder.Asc
    ) : base(page, perPage, search, sort, dir)
    { }

    public ListCategoriesQuery()
        : base(0, 15, "", "", Dto.Common.Request.SearchOrder.Asc)
    { }

    public string Type { get; set; }
}

public class ListCategoriesQueryHandler : IRequestHandler<ListCategoriesQuery, ListCategoriesOutput>
{
    private readonly ICategoryRepository categoryRepository;

    public ListCategoriesQueryHandler(ICategoryRepository categoryRepository)
        => this.categoryRepository = categoryRepository;

    public async Task<ListCategoriesOutput> Handle(
        ListCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var movementType = request.Type == MovementType.MoneyDeposit.Key.ToString() ? MovementType.MoneyDeposit : MovementType.MoneySpending;
        var searchOutput = await categoryRepository.Search(
            new(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort,
                (SearchOrder)request.Dir
            )
            {
                Type = movementType
            },
            cancellationToken
        );

        return new ListCategoriesOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items.Select(x => x.Adapt<CategoryOutput>()).ToList()
        );
    }
}