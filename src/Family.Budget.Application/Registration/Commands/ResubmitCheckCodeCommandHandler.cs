namespace Family.Budget.Application.Registration.Commands;
using Family.Budget.Application._Common;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Registrations.Errors;
using Family.Budget.Application.Dto.Registrations.Registration;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Registrations.Repository;
using FluentValidation;
using MediatR;

public record ResubmitCheckCodeCommand : IRequest<Unit>
{
    public string Email { get; set; }

    public ResubmitCheckCodeCommand(CoachRegistrationResubmitEmailInput input)
    {
        Email = input.Email;
    }
}

public class CoachRegistrationResubmitEmailInputValidator : AbstractValidator<CoachRegistrationResubmitEmailInput>
{
    public CoachRegistrationResubmitEmailInputValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField)
            .EmailAddress()
            .WithMessage(ValidationConstant.WrongEmail);

    }
}

public class ResubmitCheckCodeCommandHandler : IRequestHandler<ResubmitCheckCodeCommand, Unit>
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