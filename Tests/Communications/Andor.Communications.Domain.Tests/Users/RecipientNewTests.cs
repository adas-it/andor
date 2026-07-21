using Andor.Communications.Domain.Users;

namespace Andor.Communications.Domain.Tests.Users;

public class RecipientNewTests
{
    [Fact]
    public void New_WithValidData_ShouldCreateRecipientSuccessfully()
    {
        // Act
        var (result, recipient) = Recipient.New("John Doe", "john.doe@example.com", true, []);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(recipient);
        Assert.Equal("John Doe", recipient.Name);
        Assert.Equal("john.doe@example.com", recipient.PreferredEmail.Address);
        Assert.True(recipient.Active);
        Assert.Empty(recipient.Permissions);
    }

    [Fact]
    public void New_WithEmptyName_ShouldReturnFailure()
    {
        // Act
        var (result, recipient) = Recipient.New("", "john.doe@example.com", true, []);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(recipient);
    }

    [Fact]
    public void New_WithNameTooShort_ShouldReturnFailure()
    {
        // Act
        var (result, recipient) = Recipient.New("A", "john.doe@example.com", true, []);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(recipient);
    }

    [Fact]
    public void New_WithNameTooLong_ShouldReturnFailure()
    {
        // Act
        var (result, recipient) = Recipient.New(new string('A', 51), "john.doe@example.com", true, []);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(recipient);
    }

    [Fact]
    public void New_WithMalformedEmail_ShouldThrowFormatException()
    {
        // Act & Assert
        _ = Assert.Throws<FormatException>(() => Recipient.New("John Doe", "not-an-email", true, []));
    }

    [Fact]
    public void New_WithInactiveFlag_ShouldPreserveInactiveState()
    {
        // Act
        var (result, recipient) = Recipient.New("John Doe", "john.doe@example.com", false, []);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(recipient);
        Assert.False(recipient.Active);
    }
}
