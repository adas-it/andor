using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models;
using Andor.Application.Dto.Common.ApplicationsErrors;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Onboarding.Registrations.Requests;
using Andor.Application.Dto.Onboarding.Registrations.Responses;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andor.Application.Onboarding.Registrations.Commands;

public record ResubmitCheckCodeCommand(string Email) : IRequest<ApplicationResult<RegistrationOutput>>;

public class CoachRegistrationResubmitEmailInputValidator : AbstractValidator<ResubmitCheckCodeCommand>
{
    public CoachRegistrationResubmitEmailInputValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField)
            .EmailAddress()
            .WithMessage(ValidationConstant.InvalidParameter);
    }
}

public class ResubmitCheckCodeCommandHandler()
    : IRequestHandler<ResubmitCheckCodeCommand, Unit>
{
    private readonly IRegistrationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRequestRegistrationComunication _requestRegistrationComunication;
    private readonly Notifier _notifier;
    private readonly IKeycloackService _keycloackService;
    public ResubmitCheckCodeCommandHandler(IRegistrationRepository repository,
        IUnitOfWork unitOfWork,
        IRequestRegistrationComunication requestRegistrationComunication,
        Notifier notifier,
        IKeycloackService keycloackService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _requestRegistrationComunication = requestRegistrationComunication;
        _notifier = notifier;
        _keycloackService = keycloackService;
    }

    public async Task<Unit> Handle(ResubmitCheckCodeCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByEmail(request.Email, cancellationToken);

        if (item == null)
        {
            _notifier.Erros.Add(Errors.RegistrationNotFound());

            return Unit.Value;
        }

        var userEmail = await _keycloackService.GetUserByEmail(request.Email, cancellationToken);

        if (userEmail != null)
        {
            _notifier.Erros.Add(Errors.EmailInUse());

            return Unit.Value;
        }

        item.SetNewCode();

        await _repository.Update(item, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}
