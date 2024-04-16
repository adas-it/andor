using Andor.Application.Common.Attributes;
using Andor.Application.Common.Models;
using Andor.Application.Dto.Common.Responses;
using Andor.Application.Dto.Engagement.Budget.Account;
using Andor.Domain.Onboarding.Registrations.Repositories;
using Andor.Domain.Onboarding.Registrations.ValueObjects;
using FluentValidation;
using MediatR;

namespace Andor.Application.Engagement.Budget.Accounts.Commands;

public record CreateAccountCommand : IRequest<ApplicationResult<AccountOutput>>
{
    [SensitiveData]
    public string Email { get; set; } = "";
    public string Code { get; set; } = "";
}

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
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

public class CreateAccountCommandHandler(IQueriesRegistrationRepository _queriesRepository)
    : IRequestHandler<CreateAccountCommand, ApplicationResult<AccountOutput>>
{

    [Log]
    [Transaction]
    public async Task<ApplicationResult<AccountOutput>> Handle(CreateAccountCommand request,
        CancellationToken cancellationToken)
    {
        var response = ApplicationResult<AccountOutput>.Success();

        return response;
    }
}