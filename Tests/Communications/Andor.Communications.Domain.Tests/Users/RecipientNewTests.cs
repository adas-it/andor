using Andor.Communications.Domain.Users;
using Andor.Communications.Domain.Users.ValueObjects;

namespace Andor.Communications.Domain.Tests.Users;

public class RecipientNewTests
{
    [Fact]
    public void New_WithValidData_ShouldCreateRecipientSuccessfully()
    {
        // Act
        var (result, recipient) = Recipient.New(RecipientId.New(), "John Doe", "john.doe@example.com", "en",
            true, marketingOptIn: true, termsAndConditionsAccepted: true, privacyPolicyAccepted: true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(recipient);
        Assert.Equal("John Doe", recipient.Name);
        Assert.Equal("john.doe@example.com", recipient.Email);
        Assert.Equal("en", recipient.PreferredLanguage);
        Assert.True(recipient.Active);
        Assert.True(recipient.MarketingOptIn);
        Assert.True(recipient.TermsAndConditionsAccepted);
        Assert.True(recipient.PrivacyPolicyAccepted);
    }

    [Fact]
    public void New_WithExternalId_ShouldUseItAsTheRecipientId()
    {
        // Act - the id is meant to equal the User's own id, not a locally minted one.
        var userId = Guid.NewGuid();
        var (_, recipient) = Recipient.New(RecipientId.Load(userId), "John Doe", "john.doe@example.com", "en",
            true, true, true, true);

        // Assert
        Assert.Equal(userId, recipient!.Id.Value);
    }

    [Fact]
    public void New_WithEmptyName_ShouldReturnFailure()
    {
        // Act
        var (result, recipient) = Recipient.New(RecipientId.New(), "", "john.doe@example.com", "en",
            true, false, false, false);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(recipient);
    }

    [Fact]
    public void New_WithNameTooShort_ShouldReturnFailure()
    {
        // Act
        var (result, recipient) = Recipient.New(RecipientId.New(), "A", "john.doe@example.com", "en",
            true, false, false, false);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(recipient);
    }

    [Fact]
    public void New_WithNameTooLong_ShouldReturnFailure()
    {
        // Act
        var (result, recipient) = Recipient.New(RecipientId.New(), new string('A', 51), "john.doe@example.com", "en",
            true, false, false, false);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(recipient);
    }

    [Fact]
    public void New_WithInactiveFlag_ShouldPreserveInactiveState()
    {
        // Act
        var (result, recipient) = Recipient.New(RecipientId.New(), "John Doe", "john.doe@example.com", "en",
            false, false, false, false);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(recipient);
        Assert.False(recipient.Active);
    }

    [Fact]
    public void Update_ShouldOverwriteAllMutableFields()
    {
        // Arrange
        var (_, recipient) = Recipient.New(RecipientId.New(), "John Doe", "john.doe@example.com", "en",
            true, false, false, false);

        // Act
        var result = recipient!.Update("John D.", "new@example.com", "pt-BR", false, true, true, true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("John D.", recipient.Name);
        Assert.Equal("new@example.com", recipient.Email);
        Assert.Equal("pt-BR", recipient.PreferredLanguage);
        Assert.False(recipient.Active);
        Assert.True(recipient.MarketingOptIn);
        Assert.True(recipient.TermsAndConditionsAccepted);
        Assert.True(recipient.PrivacyPolicyAccepted);
    }
}
