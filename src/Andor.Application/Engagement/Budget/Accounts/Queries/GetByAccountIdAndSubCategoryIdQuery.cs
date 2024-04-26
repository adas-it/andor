using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.SubCategories.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Categories.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories.ValueObjects;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Queries;

public record GetByAccountIdAndSubCategoryIdQuery(AccountId AccountId,
    SubCategoryId SubCategoryId) : IRequest<ApplicationResult<SubCategoryOutput>>;

public class GetByAccountIdAndSubCategoryIdQueryHandler(IQueriesAccountSubCategoryRepository _repository)
    : IRequestHandler<GetByAccountIdAndSubCategoryIdQuery, ApplicationResult<SubCategoryOutput>>
{
    public async Task<ApplicationResult<SubCategoryOutput>> Handle(GetByAccountIdAndSubCategoryIdQuery request,
        CancellationToken cancellationToken)
    {
        var response = ApplicationResult<SubCategoryOutput>.Success();

        var item = await _repository.GetByIdAsync(request.AccountId, request.SubCategoryId, cancellationToken);

        if (item is null)
        {
            return null!;
        }

        response.SetData(item.Adapt<SubCategoryOutput>());

        return response;
    }
}
