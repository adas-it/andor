using Andor.Application.Common.Attributes;
using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Invites.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Accounts.ValueObjects;
using Andor.Domain.Engagement.Budget.Accounts.Invites;
using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Onboarding.Users.ValueObjects;
using FluentValidation;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Invites.Commands;

public record CreateInviteCommand : IRequest<ApplicationResult<InviteOutput>>
{
    public AccountId AccountId { get; set; }
    public UserId? UserId { get; set; }
    public string? Email { get; set; }
}

public class CreateInviteCommandValidator : AbstractValidator<CreateInviteCommand>
{
    public CreateInviteCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotNull()
            .WithMessage(ValidationConstant.RequiredField);
    }
}

public class CreateInviteCommandHandler(
    ICommandsInviteRepository _inviteRepository,
    IUnitOfWork _unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreateInviteCommand, ApplicationResult<InviteOutput>>
{
    [Log]
    [Transaction]
    public async Task<ApplicationResult<InviteOutput>> Handle(CreateInviteCommand request,
        CancellationToken cancellationToken)
    {
        var response = ApplicationResult<InviteOutput>.Success();

        var (_, entity) = Invite.New(request.Email, request.AccountId, currentUserService.User.UserId);

        await _inviteRepository.InsertAsync(entity, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        response.SetData(entity.Adapt<InviteOutput>());

        return response;
    }
}