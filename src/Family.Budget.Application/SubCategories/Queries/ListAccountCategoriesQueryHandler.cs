namespace Family.Budget.Application.SubCategories.Queries;

using Family.Budget.Application.Dto.Models.Request;
using Family.Budget.Application.Dto.SubCategories.Responses;
using Family.Budget.Domain.Entities.SubCategories.Repository;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Mapster;
using MediatR;

public record ListAccountCategoriesQuery
    : PaginatedListInput, IRequest<ListSubCategoriesOutput>
{
    public ListAccountCategoriesQuery(
        int page = 0,
        int perPage = 15,
        string search = "",
        string sort = "",
        Dto.Common.Request.SearchOrder dir = Dto.Common.Request.SearchOrder.Asc
    ) : base(page, perPage, search, sort, dir)
    { }

    public ListAccountCategoriesQuery()
        : base(0, 15, "", "", Dto.Common.Request.SearchOrder.Asc)
    { }

    public Guid AccountId { get; set; }
}

public class ListAccountCategoriesQueryHandler : IRequestHandler<ListAccountCategoriesQuery, ListSubCategoriesOutput>
{
    private readonly ISubCategoryRepository SubCategoryRepository;

    public ListAccountCategoriesQueryHandler(ISubCategoryRepository SubCategoryRepository)
        => this.SubCategoryRepository = SubCategoryRepository;

    public async Task<ListSubCategoriesOutput> Handle(
        ListAccountCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var searchOutput = await SubCategoryRepository.Search(
            new(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort,
                (SearchOrder)request.Dir
            ),
            cancellationToken
        );

        return new ListSubCategoriesOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(x => x.Adapt<SubCategoryOutput>())
                .ToList()
        );
    }
}