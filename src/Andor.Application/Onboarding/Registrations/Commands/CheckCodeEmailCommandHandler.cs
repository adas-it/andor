using Andor.Application.Common.Models;
using Andor.Application.Dto.Common.ApplicationsErrors;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Onboarding.Registrations.Responses;
using Andor.Domain.Entities.Onboarding.Registrations.Repositories;
using Andor.Domain.Entities.Onboarding.Registrations.ValueObjects;
using FluentValidation;
using MediatR;
using System.Net.Mail;

namespace Andor.Application.Onboarding.Registrations.Commands;

public record CheckCodeEmailCommand : IRequest<ApplicationResult<RegistrationOutput>>
{
    public string Email { get; set; }
    public string Code { get; set; }
}

public class RegistrationCheckEmailValidator : AbstractValidator<CheckCodeEmailCommand>
{
    public RegistrationCheckEmailValidator()
    {
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

    }
}

public class CheckCodeEmailCommandHandler(IQueriesRegistrationRepository _queriesRepository)
    : IRequestHandler<CheckCodeEmailCommand, ApplicationResult<RegistrationOutput>>
{
    public async Task<ApplicationResult<RegistrationOutput>> Handle(CheckCodeEmailCommand request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<RegistrationOutput>.Success();

        var email = new MailAddress(request.Email);

        var registration = await _queriesRepository.GetByEmail(email, cancellationToken);

        if (registration is null)
        {
            response.AddError(Errors.RegistrationNotFound());

            return response;
        }

        if (!registration.IsTheRightCheckCode(request.Code))
        {
            response.AddError(Errors.WrongCode());

            return response;
        }

        return response;
    }
}
