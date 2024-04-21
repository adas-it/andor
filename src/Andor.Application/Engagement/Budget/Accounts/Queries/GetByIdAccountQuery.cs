using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Account.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Queries;

public class GetByIdAccountQuery : IRequest<ApplicationResult<AccountOutput>>
{
    public AccountId AccountId { get; set; }
}

public class GetByIdAccountQueryHandler(IQueriesAccountRepository _repository) : IRequestHandler<GetByIdAccountQuery, ApplicationResult<AccountOutput>>
{
    public async Task<ApplicationResult<AccountOutput>> Handle(GetByIdAccountQuery request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<AccountOutput>.Success();

        var item = await _repository.GetByIdAsync(request.AccountId, cancellationToken);

        response.SetData(item.Adapt<AccountOutput>());

        return response;
    }
}
