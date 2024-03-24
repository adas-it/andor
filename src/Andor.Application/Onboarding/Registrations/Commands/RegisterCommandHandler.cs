using Andor.Application.Common.Attributes;
using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Onboarding.Registrations.Responses;
using Andor.Domain.Entities.Onboarding.Registrations;
using Andor.Domain.Entities.Onboarding.Registrations.Repositories;
using FluentValidation;
using MediatR;
using System.Net.Mail;

namespace Andor.Application.Onboarding.Registrations.Commands;

public record RegisterCommand : IRequest<ApplicationResult<RegistrationOutput>>
{
    [SensitiveData]
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    [SensitiveData]
    public string Email { get; set; } = string.Empty;
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
            .WithMessage(ValidationConstant.InvalidParameter);
    }
}

public class RegisterCommandHandler(
    ICommandsRegistrationRepository _repository,
    IQueriesRegistrationRepository _queriesRepository,
    IUnitOfWork _unitOfWork
    )
    : IRequestHandler<RegisterCommand, ApplicationResult<RegistrationOutput>>
{

    [Log]
    [Transaction]
    public async Task<ApplicationResult<RegistrationOutput>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var response = ApplicationResult<RegistrationOutput>.Success();

        var email = new MailAddress(request.Email);

        var entity = await _queriesRepository.GetByEmailAsync(email, cancellationToken);

        if (entity != null && entity.IsComplete())
        {
            response.AddError(Dto.Common.ApplicationsErrors.Errors.EmailInUse());
            return response;
        }

        if (entity != null && !entity.IsComplete())
        {
            response.AddError(Dto.Common.ApplicationsErrors.Errors.DuplicateRegistration());
            return response;
        }

        var (result, registration) = Registration.New(request.FirstName,
            request.LastName,
            email);

        if (result.IsFailure || registration is null)
        {
            await Errors.HandleRegistrationsResult.HandleResultConfiguration(result, response);
            return response;
        }

        await _repository.InsertAsync(registration, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return response;
    }
}
