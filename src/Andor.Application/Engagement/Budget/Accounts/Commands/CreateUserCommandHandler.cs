using Andor.Application.Common.Attributes;
using Andor.Application.Common.Interfaces;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Account.Responses;
using Andor.Application.Dto.Onboarding.Registrations.IntegrationsEvents.v1;
using Andor.Domain.Engagement.Budget.Accounts.Users;
using Andor.Domain.Engagement.Budget.Accounts.Users.Repositories;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Commands;

public record CreateUserCommand : IRequest<ApplicationResult<AccountOutput>>
{
    public UserCreated User { get; set; }
}

public class CreateUserCommandHandler(ICommandsUserRepository userRepository,
    IUnitOfWork _unitOfWork)
    : IRequestHandler<CreateUserCommand, ApplicationResult<AccountOutput>>
{
    [Log]
    [Transaction]
    public async Task<ApplicationResult<AccountOutput>> Handle(CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var response = ApplicationResult<AccountOutput>.Success();

        var (_, entity) = User.New(request.User.UserId, new System.Net.Mail.MailAddress(request.User.Email),
            request.User.FirstName,
            request.User.LastName,
            request.User.CurrencyId,
            request.User.LanguageId);

        await userRepository.InsertAsync(entity, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        response.SetData(entity.Adapt<AccountOutput>());

        return response;
    }
}