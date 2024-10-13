using Andor.Application.Common.Attributes;
using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Invites.Responses;
using Andor.Domain.Engagement.Budget.Accounts.Invites.Repositories;
using Andor.Domain.Engagement.Budget.Accounts.Invites.ValueObjects;
using FluentValidation;
using Mapster;
using MediatR;

namespace Andor.Application.Engagement.Budget.Invites.Commands;

public record AnswerInviteCommand : IRequest<ApplicationResult<InviteOutput>>
{
    public InviteId InviteId { get; set; }
    public bool IsAccepeted { get; set; }
}

public class AnswerInviteCommandValidator : AbstractValidator<AnswerInviteCommand>
{
    public AnswerInviteCommandValidator()
    {
        RuleFor(x => x.InviteId)
            .NotNull()
            .WithMessage(ValidationConstant.RequiredField);
    }
}

public class AnswerInviteCommandHandler(
    ICommandsInviteRepository _inviteRepository,
    IUnitOfWork _unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<AnswerInviteCommand, ApplicationResult<InviteOutput>>
{
    [Log]
    [Transaction]
    public async Task<ApplicationResult<InviteOutput>> Handle(AnswerInviteCommand request,
        CancellationToken cancellationToken)
    {
        var response = ApplicationResult<InviteOutput>.Success();

        var entity = await _inviteRepository.GetByIdAsync(request.InviteId, cancellationToken);

        entity.InvitationAnswered(request.IsAccepeted);

        await _unitOfWork.CommitAsync(cancellationToken);

        response.SetData(entity.Adapt<InviteOutput>());

        return response;
    }
}