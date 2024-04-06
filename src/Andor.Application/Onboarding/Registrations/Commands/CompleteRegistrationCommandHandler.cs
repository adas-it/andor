using Andor.Application.Common.Attributes;
using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Onboarding.Registrations.Responses;
using Andor.Domain.Entities.Onboarding.Registrations.Repositories;
using Andor.Domain.Entities.Onboarding.Registrations.ValueObjects;
using FluentValidation;
using Mapster;
using MediatR;
using System.Net.Mail;

namespace Andor.Application.Onboarding.Registrations.Commands;

public record CompleteRegistrationCommand : IRequest<ApplicationResult<RegistrationOutput>>
{
    public string UserName { get; set; } = "";
    [SensitiveData]
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    [SensitiveData]
    public string Email { get; set; } = "";
    public string Locale { get; set; } = "";
    public bool AcceptedTermsCondition { get; set; }
    public bool AcceptedPrivateData { get; set; }
    [SensitiveData]
    public string Password { get; set; } = "";
    public string Code { get; set; } = "";
}

public class CompleteRegistrationCommandValidator : AbstractValidator<CompleteRegistrationCommand>
{
    public CompleteRegistrationCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField)
            .Length(2, 50)
            .WithMessage(ValidationConstant.LengthError);

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

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField)
            .Length(CheckCode.MinLength, CheckCode.MaxLength)
            .WithMessage(ValidationConstant.LengthError)
            .WithMessage(ValidationConstant.LengthError);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField)
            .EmailAddress()
            .WithMessage(ValidationConstant.InvalidParameter);

        RuleFor(x => x.AcceptedTermsCondition)
            .Must(x => x)
            .WithMessage(ValidationConstant.RequiredField);

        RuleFor(x => x.AcceptedPrivateData)
            .Must(x => x)
            .WithMessage(ValidationConstant.RequiredField);
    }
}

public class CompleteRegistrationCommandHandler(ICommandsRegistrationRepository repository,
    IUnitOfWork unitOfWork,
    IQueriesRegistrationRepository queriesRepository) : IRequestHandler<CompleteRegistrationCommand,
        ApplicationResult<RegistrationOutput>>
{
    private readonly ICommandsRegistrationRepository _repository = repository;
    private readonly IQueriesRegistrationRepository _queriesRepository = queriesRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [Log]
    [Transaction]
    public async Task<ApplicationResult<RegistrationOutput>> Handle(CompleteRegistrationCommand request,
        CancellationToken cancellationToken)
    {
        var response = ApplicationResult<RegistrationOutput>.Success();

        var email = new MailAddress(request.Email);

        var registration = await _queriesRepository.GetByEmailAsync(email, cancellationToken);

        if (registration is null)
        {
            response.AddError(Dto.Common.ApplicationsErrors.Errors.RegistrationNotFound());

            return response;
        }

        if (registration.IsComplete())
        {
            response.AddError(Dto.Common.ApplicationsErrors.Errors.EmailInUse());

            return response;
        }

        if (!registration.IsTheRightCode(request.Code))
        {
            response.AddError(Dto.Common.ApplicationsErrors.Errors.WrongCode());

            return response;
        }

        registration.Complete(
            userName: request.UserName,
            firstName: request.FirstName,
            lastName: request.LastName,
            locale: request.Locale,
            acceptedTermsCondition: request.AcceptedTermsCondition,
            acceptedPrivateData: request.AcceptedPrivateData,
            password: request.Password);

        await _repository.UpdateAsync(registration, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        response.SetData(registration.Adapt<RegistrationOutput>());

        return response;
    }
}
