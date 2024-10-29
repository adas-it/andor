using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Categories.Response;
using Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;
using Andor.Domain.Engagement.Budget.FinancialMovements.MovementTypes;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Queries;

public record ListCategoriesQuery
    : PaginatedListInput, IRequest<ApplicationResult<ListCategoriesOutput>>
{
    public int Type { get; set; }
    public Domain.Engagement.Budget.Accounts.Accounts.ValueObjects.AccountId AccountId { get; set; }
}

public class ListCategoriesQueryHandler(IQueriesAccountCategoryRepository repository)
    : IRequestHandler<ListCategoriesQuery, ApplicationResult<ListCategoriesOutput>>
{
    public async Task<ApplicationResult<ListCategoriesOutput>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ListCategoriesOutput>.Success();

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

        response.SetData(searchOutput);

        return response;
    }
}
