namespace Family.Budget.Application.Accounts.Queries;

using Family.Budget.Application.Dto.Accounts.Responses;
using Family.Budget.Application.Dto.Models.Request;
using Family.Budget.Application.Models.Authorization;
using Family.Budget.Domain.Entities.Accounts.Repository;
using Family.Budget.Domain.Entities.Users;
using Family.Budget.Domain.SeedWork.ShearchableRepository;
using Mapster;
using MediatR;

public record ListAccountsQuery
    : PaginatedListInput, IRequest<ListAccountOutput>
{
    public ListAccountsQuery(
        int page = 0,
        int perPage = 15,
        string search = "",
        string sort = "",
        Dto.Common.Request.SearchOrder dir = Dto.Common.Request.SearchOrder.Asc
    ) : base(page, perPage, search, sort, dir)
    { }

    public ListAccountsQuery()
        : base(0, 15, "", "", Dto.Common.Request.SearchOrder.Asc)
    { }
}

public class ListAccountsQueryHandler : IRequestHandler<ListAccountsQuery, ListAccountOutput>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;

    public ListAccountsQueryHandler(IAccountRepository accountRepository,
        ICurrentUserService currentUserService)
    {
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ListAccountOutput> Handle(
        ListAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = (UserId)_currentUserService.User.UserId;

        var searchOutput = await _accountRepository.Search(
            new(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort,
                (SearchOrder)request.Dir,
                null,
                userId,
                null,
                null
            ),
            cancellationToken
        );

        return new ListAccountOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(x => x.Adapt<AccountOutput>())
                .ToList()
        );
    }
}