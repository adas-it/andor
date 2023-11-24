namespace Family.Budget.Application.Administrations.Commands;

using Family.Budget.Application._Common;
using FluentValidation;

public class RegisterConfigurationCommandValidator : AbstractValidator<RegisterConfigurationCommand>
{
    public RegisterConfigurationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);
    }
}