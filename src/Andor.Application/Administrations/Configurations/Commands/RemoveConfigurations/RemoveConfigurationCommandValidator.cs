using Andor.Application.Common.Models;
using FluentValidation;

namespace Andor.Application.Administrations.Configurations.Commands.RemoveConfiguration;

public class RemoveConfigurationCommandValidator : AbstractValidator<RemoveConfigurationCommand>
{
    public RemoveConfigurationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);
    }
}