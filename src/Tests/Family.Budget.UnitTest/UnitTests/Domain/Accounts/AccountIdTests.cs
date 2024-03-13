namespace Family.Budget.UnitTest.UnitTests.Domain.Accounts;
using Family.Budget.Domain.Entities.Accounts.ValueObject;
using Family.Budget.Domain.Exceptions;
using FluentAssertions;
using System;
using Xunit;

public class AccountIdTests
{
    [Fact]
    public void New_AccountId_Validation()
    {
        // Arrange
        AccountId accountId = AccountId.New();

        // Act

        // Assert
        Assert.NotNull(accountId);
    }

    [Fact]
    public void AccountId_Validation_InvalidGuid()
    {
        // Arrange
        Guid invalidGuid = Guid.NewGuid();

        // Act
        Action action =
            () => new AccountId(invalidGuid);

        //Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void AccountId_Validation_EmptyGuid()
    {
        // Arrange
        Guid invalidGuid = new();

        // Act
        Action action =
            () => new AccountId(invalidGuid);

        //Assert
        action.Should().Throw<InvalidDomainException>();
    }
}
