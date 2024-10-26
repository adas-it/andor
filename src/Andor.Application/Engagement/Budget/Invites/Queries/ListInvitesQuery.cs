using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Invites.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.SeedWork.Repositories.ResearchableRepository;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Invites.Queries;

public record ListInvitesQuery
    : PaginatedListInput, IRequest<ApplicationResult<ListInviteOutput>>;


public class ListInvitesQueryHandler(IQueriesInviteRepository _repository) : IRequestHandler<ListInvitesQuery, ApplicationResult<ListInviteOutput>>
{
    public async Task<ApplicationResult<ListInviteOutput>> Handle(ListInvitesQuery request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<ListInviteOutput>.Success();

        var item = await _repository.SearchPendingsInvitesAsync(request.Adapt<SearchInput>(), cancellationToken);

        response.SetData(item.Adapt<ListInviteOutput>());

        return response;
    }
}
