using Andor.Foundation.Domain.Validation;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Foundation.Domain.Tests.Validations;

public class DomainValidationTest
{
    [Fact(DisplayName = nameof(DateTimeNotNullSuccess))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void DateTimeNotNullSuccess()
    {
        DateTimeOffset value = DateTimeOffset.UtcNow;

        Notification act = value!.NotNull();

        _ = act.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DateTimeNotNullError))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void DateTimeNotNullError()
    {
        DateTimeOffset? value = null;

        Notification act = value!.NotNull();

        string fieldName = nameof(value);

        _ = act.Should().NotBeNull();
        _ = act!.FieldName.Should().Be(nameof(value));
        _ = act!.Message.Should().Be(DefaultsErrorsMessages.NotNull.GetMessage(fieldName));
    }

    [Fact(DisplayName = nameof(GuidNotNullSuccess))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void GuidNotNullSuccess()
    {
        var value = Guid.NewGuid();

        Notification act = value!.NotNull();

        _ = act.Should().BeNull();
    }

    [Fact(DisplayName = nameof(GuidTimeNotNullError))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void GuidTimeNotNullError()
    {
        var value = new Guid();

        Notification act = value!.NotNull();

        string fieldName = nameof(value);

        _ = act.Should().NotBeNull();
        _ = act!.FieldName.Should().Be(nameof(value));
        _ = act!.Message.Should().Be(DefaultsErrorsMessages.NotNull.GetMessage(fieldName));
    }

    [Fact(DisplayName = nameof(DateTimeNotDefaultValueError))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void DateTimeNotDefaultValueError()
    {
        var value = new DateTime();

        Notification act = value!.NotDefaultDateTime();

        string fieldName = nameof(value);

        _ = act.Should().NotBeNull();
        _ = act!.FieldName.Should().Be(nameof(value));
        _ = act!.Message.Should().Be(DefaultsErrorsMessages.NotDefaultDateTime.GetMessage(fieldName));
    }

    [Fact(DisplayName = nameof(DateTimeNotDefault))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void DateTimeNotDefault()
    {
        DateTime value = DateTime.UtcNow;

        Notification act = value.NotDefaultDateTime();

        _ = act.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DateTimeNullNotDefaultSuccess))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void DateTimeNullNotDefaultSuccess()
    {
        DateTime? value = null!;

        Notification act = value.NotDefaultDateTime();

        _ = act.Should().BeNull();
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOrWhiteSpaceSuccess))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOrWhiteSpaceSuccess()
    {
        //
        string value = "Hello";

        //Act
        Notification act = value.NotNullOrEmptyOrWhiteSpace();

        //Assert
        _ = act.Should().BeNull();
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOrWhiteSpaceErrorWithEmpty))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOrWhiteSpaceErrorWithEmpty()
    {
        //
        string value = "";

        //Act
        Notification act = value.NotNullOrEmptyOrWhiteSpace();

        //Assert
        string fieldName = nameof(value);

        _ = act.Should().NotBeNull();
        _ = act!.FieldName.Should().Be(nameof(value));
        _ = act!.Message.Should().Be(DefaultsErrorsMessages.NotNull.GetMessage(fieldName));
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOrWhiteSpaceErrorWithWhiteSpace))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOrWhiteSpaceErrorWithWhiteSpace()
    {
        //
        string value = "  ";

        //Act
        Notification act = value.NotNullOrEmptyOrWhiteSpace();

        //Assert
        string fieldName = nameof(value);

        _ = act.Should().NotBeNull();
        _ = act!.FieldName.Should().Be(nameof(value));
        _ = act!.Message.Should().Be(DefaultsErrorsMessages.NotNull.GetMessage(fieldName));
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOrWhiteSpaceErrorWithNull))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOrWhiteSpaceErrorWithNull()
    {
        //
        string value = null!;

        //Act
        Notification act = value.NotNullOrEmptyOrWhiteSpace();

        //Assert
        string fieldName = nameof(value);

        _ = act.Should().NotBeNull();
        _ = act!.FieldName.Should().Be(nameof(value));
        _ = act!.Message.Should().Be(DefaultsErrorsMessages.NotNull.GetMessage(fieldName));
    }

    //between
    [Fact(DisplayName = nameof(BetweenLengthErrorLessThenMinLength))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void BetweenLengthErrorLessThenMinLength()
    {
        //
        int minLength = 5;
        int maxLength = 10;
        string value = "".PadLeft(minLength - 1);

        //Act
        Notification act = value.BetweenLength(minLength, maxLength);

        //Assert
        string fieldName = nameof(value);

        _ = act.Should().NotBeNull();
        _ = act!.FieldName.Should().Be(nameof(value));
        _ = act!.Message.Should().Be(DefaultsErrorsMessages.BetweenLength.GetMessage(fieldName, minLength, maxLength));
    }

    [Fact(DisplayName = nameof(BetweenLengthErrorMoreThenMaxLength))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void BetweenLengthErrorMoreThenMaxLength()
    {
        //
        int minLength = 5;
        int maxLength = 10;
        string value = "".PadLeft(maxLength + 1);

        //Act
        Notification act = value.BetweenLength(minLength, maxLength);

        //Assert
        string fieldName = nameof(value);

        _ = act.Should().NotBeNull();
        _ = act!.FieldName.Should().Be(nameof(value));
        _ = act!.Message.Should().Be(DefaultsErrorsMessages.BetweenLength.GetMessage(fieldName, minLength, maxLength));
    }

    [Fact(DisplayName = "BetweenLengthSuccess")]
    [Trait("Domain", "DomainValidation - Validation")]
    public void BetweenLengthSuccessExactMinLength()
    {
        //
        int minLength = 5;
        int maxLength = 10;
        string value = "".PadLeft(minLength);

        //Act
        Notification act = value.BetweenLength(minLength, maxLength);

        //Assert
        _ = act.Should().BeNull();
    }

    [Fact(DisplayName = "BetweenLengthSuccess")]
    [Trait("Domain", "DomainValidation - Validation")]
    public void BetweenLengthSuccessExactMaxLength()
    {
        //
        int minLength = 5;
        int maxLength = 10;
        string value = "".PadLeft(maxLength);

        //Act
        Notification act = value.BetweenLength(minLength, maxLength);

        //Assert
        _ = act.Should().BeNull();
    }

    [Fact(DisplayName = "BetweenLengthSuccess")]
    [Trait("Domain", "DomainValidation - Validation")]
    public void BetweenLengthSuccess()
    {
        //
        int minLength = 5;
        int maxLength = 10;
        string value = "".PadLeft(maxLength - 2);

        //Act
        Notification act = value.BetweenLength(minLength, maxLength);

        //Assert
        _ = act.Should().BeNull();
    }

    [Fact(DisplayName = "BetweenLengthNotValidNull")]
    [Trait("Domain", "DomainValidation - Validation")]
    public void BetweenLengthNotValidNull()
    {
        //
        int minLength = 5;
        int maxLength = 10;
        string value = null!;

        //Act
        Notification act = value.BetweenLength(minLength, maxLength);

        //Assert
        _ = act.Should().BeNull();
    }
}
