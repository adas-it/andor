using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Account.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Queries;

public record ListAccountsQuery
    : PaginatedListInput, IRequest<ApplicationResult<ListAccountOutput>>;


public class ListAccountsQueryHandler(IQueriesAccountRepository _repository) : IRequestHandler<ListAccountsQuery, ApplicationResult<ListAccountOutput>>
{
    public async Task<ApplicationResult<ListAccountOutput>> Handle(ListAccountsQuery request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ListAccountOutput>.Success();

        var item = await _repository.SearchAsync(request.Adapt<SearchInput>(), cancellationToken);

        response.SetData(item.Adapt<ListAccountOutput>());

        return response;
    }
}
