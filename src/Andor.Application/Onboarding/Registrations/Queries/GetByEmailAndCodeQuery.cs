using Andor.Application.Common.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Onboarding.Registrations.Responses;
using Andor.Domain.Entities.Onboarding.Registrations.Repositories;
using Andor.Domain.Entities.Onboarding.Registrations.ValueObjects;
using FluentValidation;
using Mapster;
using MediatR;
using System.Net.Mail;

namespace Andor.Application.Onboarding.Registrations.Queries;

public record GetByEmailAndCodeQuery : IRequest<ApplicationResult<RegistrationOutput>>
{
    public string Email { get; set; } = "";
    public string Code { get; set; } = "";
}

public class GetByEmailAndCodeQueryValidator : AbstractValidator<GetByEmailAndCodeQuery>
{
    public GetByEmailAndCodeQueryValidator()
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

public class GetByEmailAndCodeQueryHandler(IQueriesRegistrationRepository _queriesRepository)
    : IRequestHandler<GetByEmailAndCodeQuery, ApplicationResult<RegistrationOutput>>
{
    public async Task<ApplicationResult<RegistrationOutput>> Handle(GetByEmailAndCodeQuery request,
        CancellationToken cancellationToken)
    {
        var response = ApplicationResult<RegistrationOutput>.Success();

        var registration = await _queriesRepository.GetByEmailAsync(new MailAddress(request.Email), cancellationToken);

        if (registration is null)
        {
            response.AddError(Dto.Common.ApplicationsErrors.Errors.RegistrationNotFound());

            return response;
        }

        if (!registration.IsTheRightCode(request.Code))
        {
            response.AddError(Dto.Common.ApplicationsErrors.Errors.WrongCode());

            return response;
        }

        response.SetData(registration.Adapt<RegistrationOutput>());

        return response;
    }
}