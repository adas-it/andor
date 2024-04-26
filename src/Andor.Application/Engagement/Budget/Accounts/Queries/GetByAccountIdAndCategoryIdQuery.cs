using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Categories.Response;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Categories.ValueObjects;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Queries;

public record GetByAccountIdAndCategoryIdQuery(AccountId AccountId,
    CategoryId CategoryId) : IRequest<ApplicationResult<CategoryOutput>>;

public class GetByAccountIdAndCategoryIdQueryHandler(IQueriesAccountCategoryRepository _repository)
    : IRequestHandler<GetByAccountIdAndCategoryIdQuery, ApplicationResult<CategoryOutput>>
{
    public async Task<ApplicationResult<CategoryOutput>> Handle(GetByAccountIdAndCategoryIdQuery request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<CategoryOutput>.Success();

        var item = await _repository.GetByIdAsync(request.AccountId, request.CategoryId, cancellationToken);

        if (item is null)
        {
            return null!;
        }

        response.SetData(item.Adapt<CategoryOutput>());

        return response;
    }
}
