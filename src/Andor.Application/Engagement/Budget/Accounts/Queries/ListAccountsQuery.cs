using Andor.Application.Dto.Common.Requests;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Account.Responses;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Queries;

public record ListAccountsQuery
    : PaginatedListInput, IRequest<ApplicationResult<ListAccountOutput>>;
