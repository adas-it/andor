﻿using Andor.Application.Common.Models;
using FluentValidation;

namespace Andor.Application.Administrations.Configurations.Commands.ModifyConfiguration;

public class ModifyConfigurationCommandValidator : AbstractValidator<ModifyConfigurationCommand>
{
    public ModifyConfigurationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);

        RuleFor(x => x.PatchDocument)
            .NotEmpty()
            .WithMessage(ValidationConstant.RequiredField);
    }
}
