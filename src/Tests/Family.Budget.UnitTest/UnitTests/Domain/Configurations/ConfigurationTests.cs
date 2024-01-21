namespace Family.Budget.UnitTest.UnitTests.Domain.Configurations;

using Family.Budget.Domain.Common;
using Family.Budget.Domain.Exceptions;
using FluentAssertions;
using System;
using Xunit;
using DomainEntity = Family.Budget.Domain.Entities.Admin;

[Collection(nameof(ConfigurationTestFixture))]
public class ConfigurationTests
{
    private readonly ConfigurationTestFixture fixture;

    public ConfigurationTests(ConfigurationTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instatiate))]
    [Trait("Domain", "Configuration - Aggregates")]
    public void Instatiate()
    {
        //Act
        var datetimeBefore = DateTime.UtcNow;

        var validData = fixture.GetValidConfiguration();

        var datetimeAfter = DateTime.UtcNow.AddSeconds(1);

        //Assert
        validData.Should().NotBeNull();
        validData.Id.Should().NotBeEmpty();
        (validData.StartDate > datetimeBefore).Should().BeTrue();
    }

    [Fact(DisplayName = nameof(InstatiateRequeridFieldNameNull))]
    [Trait("Domain", "Configuration - Aggregates")]
    public void InstatiateRequeridFieldNameNull()
    {
        //Arrange
        var validData = fixture.GetValidConfiguration();

        //Act
        Action action =
            () => DomainEntity.Configuration.New(null!,
            validData.Value,
            validData.Description,
            validData.StartDate,
            validData.FinalDate,
            Guid.NewGuid().ToString());

        //Assert
        var msg = $"{CommonErrorCodes.Validation.Value}:{DefaultsErrorsMessages.NotNull.GetMessage(nameof(DomainEntity.Configuration.Name))}";

        action.Should().Throw<InvalidDomainException>()
            .WithMessage(msg);
    }

    [Fact(DisplayName = nameof(InstatiateRequeridFieldValueNull))]
    [Trait("Domain", "Configuration - Aggregates")]
    public void InstatiateRequeridFieldValueNull()
    {
        //Arrange
        var validData = fixture.GetValidConfiguration();

        //Act
        Action action =
            () =>  DomainEntity.Configuration.New(validData.Name,
            null!,
            validData.Description,
            validData.StartDate,
            validData.FinalDate,
            Guid.NewGuid().ToString());

        //Assert
        var msg = $"{CommonErrorCodes.Validation.Value}:{DefaultsErrorsMessages.NotNull.GetMessage(nameof(DomainEntity.Configuration.Value))}";

        action.Should().Throw<InvalidDomainException>()
            .WithMessage(msg);
    }

    [Fact(DisplayName = nameof(InstatiateRequeridFieldDescriptionNull))]
    [Trait("Domain", "Configuration - Aggregates")]
    public void InstatiateRequeridFieldDescriptionNull()
    {
        //Arrange
        var validData = fixture.GetValidConfiguration();

        //Act
        Action action =
            () =>  DomainEntity.Configuration.New(validData.Name,
            validData.Value,
            null!,
            validData.StartDate,
            validData.FinalDate,
            Guid.NewGuid().ToString());

        //Assert
        var msg = $"{CommonErrorCodes.Validation.Value}:{DefaultsErrorsMessages.NotNull.GetMessage(nameof(DomainEntity.Configuration.Description))}";

        action.Should().Throw<InvalidDomainException>()
            .WithMessage(msg);
    }

    [Fact(DisplayName = nameof(InstatiateRequeridFieldStartDateNull))]
    [Trait("Domain", "Configuration - Aggregates")]
    public void InstatiateRequeridFieldStartDateNull()
    {
        //Arrange
        var validData = fixture.GetValidConfiguration();

        //Act
        Action action =
            () =>  DomainEntity.Configuration.New(validData.Name,
            validData.Value,
            validData.Description,
            DateTimeOffset.MinValue,
            validData.FinalDate,
            Guid.NewGuid().ToString());

        //Assert
        var msg = $"{CommonErrorCodes.Validation.Value}:{DefaultsErrorsMessages.NotDefaultDateTime.GetMessage(nameof(DomainEntity.Configuration.StartDate))}";

        action.Should().Throw<InvalidDomainException>()
            .WithMessage(msg);
    }

    //[Fact(DisplayName = nameof(InstatiateRequeridFieldFinalDateNull))]
    //[Trait("Domain", "Configuration - Aggregates")]
    public void InstatiateRequeridFieldFinalDateNull()
    {
        //Arrange
        var validData = fixture.GetValidConfiguration();

        //Act
        Action action =
            () =>  DomainEntity.Configuration.New(validData.Name,
            validData.Value,
            validData.Description,
            validData.StartDate,
            DateTimeOffset.MinValue,
            Guid.NewGuid().ToString());

        //Assert
        var msg = $"{CommonErrorCodes.Validation.Value}:{DefaultsErrorsMessages.NotDefaultDateTime.GetMessage(nameof(DomainEntity.Configuration.FinalDate))}";

        action.Should().Throw<InvalidDomainException>()
            .WithMessage(msg);
    }
}
