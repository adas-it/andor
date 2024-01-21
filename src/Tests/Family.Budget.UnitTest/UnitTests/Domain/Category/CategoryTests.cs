namespace Family.Budget.UnitTest.UnitTests.Domain.Category;

using Budget.Domain.Entities.Categories;
using Family.Budget.Domain.Common;
using Family.Budget.Domain.Entities.FinancialMovement.MovementTypes;
using Family.Budget.Domain.Exceptions;
using FluentAssertions;
using System;
using Xunit;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTests
{
    private readonly CategoryTestFixture fixture;

    public CategoryTests(CategoryTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instatiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instatiate()
    {
        //Act
        var validData = fixture.GetValidCategory();

        //Assert
        validData.Should().NotBeNull();
        validData.Id.Should().NotBeEmpty();
        validData.Events.Should().NotBeEmpty();
    }

    [Fact(DisplayName = nameof(InstatiateOnlyDeactivateDateSet))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstatiateOnlyDeactivateDateSet()
    {
        //Arrange
        var validData = fixture.GetValidCategory();

        //Act
        Action action =
            () => Category.New(validData.Name,
            validData.Description,
            null!,
            validData.DeactivationDate,
            MovementType.MoneySpending);

        //Assert
        action.Should().NotThrow();
    }

    //[Fact(DisplayName = nameof(InstatiateRequeridFieldNameNull))]
    //[Trait("Domain", "Category - Aggregates")]
    public void InstatiateRequeridFieldNameNull()
    {
        //Arrange
        var validData = fixture.GetValidCategory();

        //Act
        Action action =
            () => Category.New(null!,
            validData.Description,
            validData.StartDate,
            validData.DeactivationDate,
            MovementType.MoneySpending);

        //Assert
        var msg = $"{(int)Budget.Domain.Entities.Admin.ConfigurationsErrorsCodes.Validation.Value}:{DefaultsErrorsMessages.NotNull.GetMessage(nameof(Category.Name))}";

        action.Should().Throw<InvalidDomainException>()
            .WithMessage(msg);
    }

    //[Fact(DisplayName = nameof(InstatiateRequeridFieldDescriptionNull))]
    //[Trait("Domain", "Category - Aggregates")]
    public void InstatiateRequeridFieldDescriptionNull()
    {
        //Arrange
        var validData = fixture.GetValidCategory();

        //Act
        Action action =
            () => Category.New(validData.Name,
            null!,
            validData.StartDate,
            validData.DeactivationDate,
            MovementType.MoneySpending);

        //Assert
        var msg = $"{(int)Budget.Domain.Entities.Admin.ConfigurationsErrorsCodes.Validation.Value}:{DefaultsErrorsMessages.NotNull.GetMessage(nameof(Category.Description))}";

        action.Should().Throw<InvalidDomainException>()
            .WithMessage(msg);
    }

    //[Fact(DisplayName = nameof(InstatiateRequeridFieldStartDateDefaultDate))]
    //[Trait("Domain", "Category - Aggregates")]
    public void InstatiateRequeridFieldStartDateDefaultDate()
    {
        //Arrange
        var validData = fixture.GetValidCategory();

        //Act
        Action action =
            () => Category.New(validData.Name,
            validData.Description,
            new DateTimeOffset(),
            validData.DeactivationDate,
            MovementType.MoneySpending);

        //Assert
        var msg = $"{(int)Budget.Domain.Entities.Admin.ConfigurationsErrorsCodes.Validation.Value}:{DefaultsErrorsMessages.NotDefaultDateTime.GetMessage(nameof(Category.StartDate))}";

        action.Should().Throw<InvalidDomainException>()
            .WithMessage(msg);
    }

    //[Fact(DisplayName = nameof(InstatiateRequeridFieldDeactivationDateDefaultDate))]
    //[Trait("Domain", "Category - Aggregates")]
    public void InstatiateRequeridFieldDeactivationDateDefaultDate()
    {
        //Arrange
        var validData = fixture.GetValidCategory();

        //Act
        Action action =
            () => Category.New(validData.Name,
            validData.Description,
            validData.StartDate,
            new DateTimeOffset(),
            MovementType.MoneySpending);

        //Assert
        var msg = $"{(int)Budget.Domain.Entities.Admin.ConfigurationsErrorsCodes.Validation.Value}:{DefaultsErrorsMessages.NotDefaultDateTime.GetMessage(nameof(Category.DeactivationDate))}";

        action.Should().Throw<InvalidDomainException>()
            .WithMessage(msg);
    }

    //[Fact(DisplayName = nameof(InstatiateRequeridFieldDeactivationDateBeforeStartDate))]
    //[Trait("Domain", "Category - Aggregates")]
    public void InstatiateRequeridFieldDeactivationDateBeforeStartDate()
    {
        //Arrange
        var validData = fixture.GetValidCategory();

        //Act
        Action action =
            () => Category.New(validData.Name,
            validData.Description,
            validData.StartDate,
            validData.StartDate!.Value.AddDays(-1),
            MovementType.MoneySpending);

        //Assert
        var msg = @$"{(int)Budget.Domain.Entities.Admin.ConfigurationsErrorsCodes.Validation.Value}:{DefaultsErrorsMessages.Date0CannotBeBeforeDate1
            .GetMessage(nameof(Category.DeactivationDate), nameof(Category.StartDate))}";

        action.Should().Throw<InvalidDomainException>()
            .WithMessage(msg);
    }
}