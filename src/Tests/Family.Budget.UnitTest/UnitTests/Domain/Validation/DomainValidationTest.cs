namespace Family.Budget.UnitTest.UnitTests.Domain.Validation;

using Family.Budget.Domain.Common;
using Family.Budget.Domain.Exceptions;
using Family.Budget.Domain.Validation;
using FluentAssertions;
using System;
using Xunit;

public class DomainValidationTest
{
    [Fact(DisplayName = nameof(DateTimeNotNullSuccess))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void DateTimeNotNullSuccess()
    {
        var value = DateTimeOffset.UtcNow;

        var act = value!.NotNull();

        act.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DateTimeNotNullError))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void DateTimeNotNullError()
    {
        DateTimeOffset? value = null;

        var act = value!.NotNull();

        var fieldName = nameof(value);

        act.Should().NotBeNull();
        act!.FieldName.Should().Be(nameof(value));
        act!.Message.Should().Be(DefaultsErrorsMessages.NotNull.GetMessage(fieldName));
    }

    [Fact(DisplayName = nameof(GuidNotNullSuccess))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void GuidNotNullSuccess()
    {
        var value = Guid.NewGuid();

        var act = value!.NotNull();

        act.Should().BeNull();
    }

    [Fact(DisplayName = nameof(GuidTimeNotNullError))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void GuidTimeNotNullError()
    {
        var value = new Guid();

        var act = value!.NotNull();

        var fieldName = nameof(value);

        act.Should().NotBeNull();
        act!.FieldName.Should().Be(nameof(value));
        act!.Message.Should().Be(DefaultsErrorsMessages.NotNull.GetMessage(fieldName));
    }

    [Fact(DisplayName = nameof(DateTimeNotDefaultValueError))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void DateTimeNotDefaultValueError()
    {
        var value = new DateTimeOffset();

        var act = value!.NotDefaultDateTime();

        var fieldName = nameof(value);

        act.Should().NotBeNull();
        act!.FieldName.Should().Be(nameof(value));
        act!.Message.Should().Be(DefaultsErrorsMessages.NotDefaultDateTime.GetMessage(fieldName));
    }

    [Fact(DisplayName = nameof(DateTimeNotDefault))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void DateTimeNotDefault()
    {
        var value = DateTimeOffset.UtcNow;

        var act = value.NotDefaultDateTime();

        act.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DateTimeNUllNotDefaultSuccess))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void DateTimeNUllNotDefaultSuccess()
    {
        DateTimeOffset? value = null!;

        var act = value.NotDefaultDateTime();

        act.Should().BeNull();
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOrWhiteSpaceSuccess))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOrWhiteSpaceSuccess()
    {
        //
        string value = "Hello";

        //Act
        var act = value.NotNullOrEmptyOrWhiteSpace();

        //Assert
        act.Should().BeNull();
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOrWhiteSpaceErrorWithEmpty))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOrWhiteSpaceErrorWithEmpty()
    {
        //
        string value = "";

        //Act
        var act = value.NotNullOrEmptyOrWhiteSpace();

        //Assert
        var fieldName = nameof(value);

        act.Should().NotBeNull();
        act!.FieldName.Should().Be(nameof(value));
        act!.Message.Should().Be(DefaultsErrorsMessages.NotNull.GetMessage(fieldName));
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOrWhiteSpaceErrorWithWhiteSpace))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOrWhiteSpaceErrorWithWhiteSpace()
    {
        //
        string value = "  ";

        //Act
        var act = value.NotNullOrEmptyOrWhiteSpace();

        //Assert
        var fieldName = nameof(value);

        act.Should().NotBeNull();
        act!.FieldName.Should().Be(nameof(value));
        act!.Message.Should().Be(DefaultsErrorsMessages.NotNull.GetMessage(fieldName));
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOrWhiteSpaceErrorWithNull))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOrWhiteSpaceErrorWithNull()
    {
        //
        string value = null!;

        //Act
        var act = value.NotNullOrEmptyOrWhiteSpace();

        //Assert
        var fieldName = nameof(value);

        act.Should().NotBeNull();
        act!.FieldName.Should().Be(nameof(value));
        act!.Message.Should().Be(DefaultsErrorsMessages.NotNull.GetMessage(fieldName));
    }

    //between
    [Fact(DisplayName = nameof(BetweenLengthErrorLessThenMinLength))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void BetweenLengthErrorLessThenMinLength()
    {
        //
        var minLength = 5;
        var maxLength = 10;
        string value = "".PadLeft(minLength -1);

        //Act
        var act = value.BetweenLength(minLength, maxLength);

        //Assert
        var fieldName = nameof(value);

        act.Should().NotBeNull();
        act!.FieldName.Should().Be(nameof(value));
        act!.Message.Should().Be(DefaultsErrorsMessages.BetweenLength.GetMessage(fieldName, minLength, maxLength));
    }

    [Fact(DisplayName = nameof(BetweenLengthErrorMoreThenMaxLength))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void BetweenLengthErrorMoreThenMaxLength()
    {
        //
        var minLength = 5;
        var maxLength = 10;
        string value = "".PadLeft(maxLength + 1);

        //Act
        var act = value.BetweenLength(minLength, maxLength);

        //Assert
        var fieldName = nameof(value);

        act.Should().NotBeNull();
        act!.FieldName.Should().Be(nameof(value));
        act!.Message.Should().Be(DefaultsErrorsMessages.BetweenLength.GetMessage(fieldName, minLength, maxLength));
    }

    [Fact(DisplayName = "BetweenLengthSuccess")]
    [Trait("Domain", "DomainValidation - Validation")]
    public void BetweenLengthSuccessExactMinLength()
    {
        //
        var minLength = 5;
        var maxLength = 10;
        string value = "".PadLeft(minLength);

        //Act
        var act = value.BetweenLength(minLength, maxLength);

        //Assert
        act.Should().BeNull();
    }

    [Fact(DisplayName = "BetweenLengthSuccess")]
    [Trait("Domain", "DomainValidation - Validation")]
    public void BetweenLengthSuccessExactMaxLength()
    {
        //
        var minLength = 5;
        var maxLength = 10;
        string value = "".PadLeft(maxLength);

        //Act
        var act = value.BetweenLength(minLength, maxLength);

        //Assert
        act.Should().BeNull();
    }

    [Fact(DisplayName = "BetweenLengthSuccess")]
    [Trait("Domain", "DomainValidation - Validation")]
    public void BetweenLengthSuccess()
    {
        //
        var minLength = 5;
        var maxLength = 10;
        string value = "".PadLeft(maxLength - 2);

        //Act
        var act = value.BetweenLength(minLength, maxLength);

        //Assert
        act.Should().BeNull();
    }

    [Fact(DisplayName = "BetweenLengthNotValidNull")]
    [Trait("Domain", "DomainValidation - Validation")]
    public void BetweenLengthNotValidNull()
    {
        //
        var minLength = 5;
        var maxLength = 10;
        string value = null!;

        //Act
        var act = value.BetweenLength(minLength, maxLength);

        //Assert
        act.Should().BeNull();
    }
}