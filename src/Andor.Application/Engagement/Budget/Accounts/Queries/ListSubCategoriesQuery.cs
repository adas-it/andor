using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.SubCategories.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Categories.ValueObjects;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Queries;

public record ListSubCategoriesQuery
    : PaginatedListInput, IRequest<ApplicationResult<ListSubCategoriesOutput>>
{
    public CategoryId? CategoryId { get; set; }
    public AccountId AccountId { get; set; }
}

public class ListSubCategoriesQueryHandler(IQueriesAccountSubCategoryRepository repository)
    : IRequestHandler<ListSubCategoriesQuery, ApplicationResult<ListSubCategoriesOutput>>
{
    public async Task<ApplicationResult<ListSubCategoriesOutput>> Handle(ListSubCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ListSubCategoriesOutput>.Success();

        if (request.CategoryId is null)
        {
            return response;
        }

        var searchOutput = await repository.SearchAsync(
            new(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort,
                (Domain.SeedWork.Repositories.ResearchableRepository.SearchOrder)request.Dir,
                request.CategoryId,
                request.AccountId
            ),
            cancellationToken
        );

        var output = new ListSubCategoriesOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items.Select(x => x.Adapt<SubCategoryOutput>()).ToList()
        );

        response.SetData(output);

        return response;
    }
}
