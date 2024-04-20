using Andor.Application.Common.Attributes;
using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Onboarding.Registrations.Responses;
using Andor.Domain.Communications.Repositories;
using Andor.Domain.Entities.Admin.Configurations.Repository;
using Andor.Domain.Onboarding.Registrations;
using Andor.Domain.Onboarding.Registrations.Repositories;
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
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
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
    IQueriesLanguageRepository _queriesLanguageRepository,
    IQueriesCurrencyRepository _queriesCurrencyRepository,
    IQueriesConfigurationRepository _queriesConfigurationRepository,
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

        var defaultCurrencyId = await _queriesConfigurationRepository.GetActiveByNameAsync("defaultLocation:currency", cancellationToken);
        var defaultLanguageId = await _queriesConfigurationRepository.GetActiveByNameAsync("defaultLocation:language", cancellationToken);
        var defaultCountryId = await _queriesConfigurationRepository.GetActiveByNameAsync("defaultLocation:country", cancellationToken);

        var defaultCurrency = await _queriesCurrencyRepository.GetByIdAsync(Guid.Parse(defaultCurrencyId.Value), cancellationToken);
        var defaultLanguage = await _queriesLanguageRepository.GetByIdAsync(Guid.Parse(defaultLanguageId.Value), cancellationToken);

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

        var (result, registration) = Registration
            .New(request.FirstName,
            request.LastName,
            email,
            defaultLanguage!,
            defaultCurrency!,
            Guid.Parse(defaultCountryId.Value));

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
