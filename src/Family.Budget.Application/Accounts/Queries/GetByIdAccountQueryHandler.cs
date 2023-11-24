namespace Family.Budget.Application.Accounts.Queries;
using Family.Budget.Application.Dto.Accounts.Responses;
using Family.Budget.Application.Dto.Accounts.Errors;
using Family.Budget.Application.Models;
using Family.Budget.Application.Models.Authorization;
using Family.Budget.Domain.Entities.Accounts.Repository;
using Mapster;
using MediatR;
using Family.Budget.Application;
using Family.Budget.Domain.Entities.Accounts.ValueObject;
using Family.Budget.Domain.Entities.Users;

public class GetByIdAccountQuery : IRequest<AccountOutput>
{
    public AccountId AccountId { get; set; }
}

public class GetByIdAccountQueryHandler : BaseCommands, IRequestHandler<GetByIdAccountQuery, AccountOutput>
{
    private readonly IAccountRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetByIdAccountQueryHandler(IAccountRepository repository,
        ICurrentUserService currentUserService,
        Notifier notifier) : base(notifier)
    {
        _currentUserService = currentUserService;
        _repository = repository;
    }

    public async Task<AccountOutput> Handle(GetByIdAccountQuery request, CancellationToken cancellationToken)
    {
        var userId = (UserId)_currentUserService.User.UserId;

        var item = await _repository.GetByIdandUser(request.AccountId, userId, cancellationToken);

        if (item is null)
        {
            _notifier.Warnings.Add(AccountError.AccountNotFound());

            return null!;
        }

        return item.Adapt<AccountOutput>();
    }
}