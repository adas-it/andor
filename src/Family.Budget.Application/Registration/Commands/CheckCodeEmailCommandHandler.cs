namespace Family.Budget.Application.Registration.Commands;

using Family.Budget.Application._Common;
using Family.Budget.Application.Dto.Registrations.Errors;
using Family.Budget.Application.Dto.Registrations.Registration;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Registrations.Repository;
using FluentValidation;
using MediatR;
using System.Threading.Tasks;

public record CheckCodeEmailCommand : IRequest<Unit?>
{
    public string Email { get; set; }
    public string Code { get; set; }
}

public class RegistrationCheckEmailValidator : AbstractValidator<RegistrationCheckEmail>
{
    public RegistrationCheckEmailValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField)
            .Length(4)
            .WithMessage(ValidationConstant.LengthError);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField)
            .EmailAddress()
            .WithMessage(ValidationConstant.WrongEmail);

    }
}

public class CheckCodeEmailCommandHandler : IRequestHandler<CheckCodeEmailCommand, Unit?>
{
    private readonly IRegistrationRepository _repository;
    private readonly Notifier _notifier;
    public CheckCodeEmailCommandHandler(IRegistrationRepository repository, Notifier notifier)
    {
        _repository = repository;
        _notifier = notifier;
    }

    public async Task<Unit?> Handle(CheckCodeEmailCommand request, CancellationToken cancellationToken)
    {
        var registration = await _repository.GetByEmail(request.Email, cancellationToken);

        if (registration is null)
        {
            _notifier.Erros.Add(Errors.RegistrationNotFound());
            return null;
        }

        if (!registration.IsTheRightCheckCode(request.Code))
        {
            _notifier.Erros.Add(Errors.WrongCode());
            return null;
        }

        return Unit.Value;
    }
}
