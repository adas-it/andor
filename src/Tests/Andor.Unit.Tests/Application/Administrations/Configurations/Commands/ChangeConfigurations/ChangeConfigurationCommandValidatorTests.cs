﻿// Ignore Spelling: Validator

using Andor.Application.Administrations.Configurations.Commands.ChangeConfigurations;
using Andor.Domain.Administrations.Configurations.ValueObjects;
using Andor.TestsUtil;
using Andor.Unit.Tests.Domain.Entities.Admin.Configurations;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Andor.Unit.Tests.Application.Administrations.Configurations.Commands.ChangeConfigurations;

[Collection(nameof(ConfigurationTestFixture))]
public class ChangeConfigurationCommandValidatorTests()
{
    private readonly ChangeConfigurationCommandValidator _validator = new();

    [Fact]
    public void Name_ShouldHaveError_WhenNameIsEmpty()
    {
        var command = GetCommand(ConfigurationId.New(), name: "", value: ConfigurationFixture.GetValidValue());
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(config => config.Name);
    }

    [Fact]
    public void Value_ShouldHaveError_WhenValueIsEmpty()
    {
        var command = GetCommand(ConfigurationId.New(), name: ConfigurationFixture.GetValidName(), value: "");
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(config => config.Value);
    }

    [Fact]
    public void ShouldNotHaveError_WhenNameAndValueAreNotEmpty()
    {
        var command = GetCommand(ConfigurationId.New(), name: ConfigurationFixture.GetValidName(), value: ConfigurationFixture.GetValidValue());
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ShouldNotHaveError_WhenIdEmpty()
    {
        var command = GetCommand(Guid.Empty, name: ConfigurationFixture.GetValidName(), value: ConfigurationFixture.GetValidValue());
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(config => config.Id);
    }

    private static ChangeConfigurationCommand GetCommand(ConfigurationId Id, string? name, string? value)
        => new(Id, ConfigurationTestFixture.GetBaseConfiguration(name, value));
}
