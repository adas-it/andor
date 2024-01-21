namespace Family.Budget.UnitTest.UnitTests.Domain.Exceptions;

using Family.Budget.Domain.Exceptions;
using FluentAssertions;
using System;
using Xunit;

public class EntityGenericExceptionTests
{
    [Fact(DisplayName = nameof(InstatiateWithMessageAndCode))]
    [Trait("Domain", "Exceptions - EntityGenericException")]
    public void InstatiateWithMessageAndCode()
    {
        //Arrange
        var msg = "New Exception";
        var ErrorCode = Budget.Domain.Entities.Admin.ConfigurationsErrorsCodes.Validation;

        //Act
        var ex = new InvalidDomainException(msg, ErrorCode);

        //Assert
        ex.Message.Should().Be(msg);
        ex.Code.Should().Be(ErrorCode.Value);
    }

    [Fact(DisplayName = nameof(ThrowWithMessageAndCode))]
    [Trait("Domain", "Exceptions - EntityGenericException")]
    public void ThrowWithMessageAndCode()
    {
        //Arrange
        var msg = "New Exception";

        //Act
        Action action = () => throw new InvalidDomainException(msg, Budget.Domain.Entities.Admin.ConfigurationsErrorsCodes.Validation);

        //Assert
        action.Should().Throw<InvalidDomainException>()
                .WithMessage(msg);
    }
}