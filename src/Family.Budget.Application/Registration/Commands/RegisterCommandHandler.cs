namespace Family.Budget.Application.Registration.Commands;

using Family.Budget.Application._Common;
using Family.Budget.Application.Common.Attributes;
using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Registrations.Errors;
using Family.Budget.Application.Models;
using Family.Budget.Domain.Entities.Registrations;
using Family.Budget.Domain.Entities.Registrations.Repository;
using FluentValidation;
using MediatR;
using System.Threading.Tasks;

public record RegisterCommand : IRequest<Unit>
{

    [SensitiveData]
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [SensitiveData]
    public string Email { get; set; }
}

public class RegistrationValidator : AbstractValidator<RegisterCommand>
{
    public RegistrationValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField)
            .Length(2, 50)
            .WithMessage(ValidationConstant.LengthError);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField)
            .Length(2, 50)
            .WithMessage(ValidationConstant.LengthError);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField)
            .EmailAddress()
            .WithMessage(ValidationConstant.WrongEmail);

    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
{
    private readonly IRegistrationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly Notifier _notifier;
    private readonly IKeycloackService _keycloackService;
    public RegisterCommandHandler(IRegistrationRepository repository,
        IUnitOfWork unitOfWork,
        Notifier notifier,
        IKeycloackService keycloackService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _notifier = notifier;
        _keycloackService = keycloackService;
    }

    [Log]
    [Transaction]
    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByEmail(request.Email, cancellationToken);

        if (item != null)
        {
            _notifier.Erros.Add(Errors.DuplicateRegistration());

            return Unit.Value;
        }

        var userEmail = await _keycloackService.GetUserByEmail(request.Email, cancellationToken);

        if (userEmail != null)
        {
            _notifier.Erros.Add(Errors.EmailInUse());

            return Unit.Value;
        }

        item = Registration.New(request.FirstName,
            request.LastName,
            request.Email);

        await _repository.Insert(item, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}
