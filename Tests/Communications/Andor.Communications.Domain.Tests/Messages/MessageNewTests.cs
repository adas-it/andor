using Andor.Communications.Domain.Messages;
using Andor.Communications.Domain.Users.ValueObjects;

namespace Andor.Communications.Domain.Tests.Messages;

public class MessageNewTests
{
    [Fact]
    public void New_WithValidData_ShouldCreateMessageSuccessfully()
    {
        // Act
        var recipientId = RecipientId.New();
        var (result, message) = Message.New(recipientId, "Welcome!", "Thanks for signing up.");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(message);
        Assert.Equal(recipientId, message.RecipientId);
        Assert.Equal("Welcome!", message.Title);
        Assert.Equal("Thanks for signing up.", message.Body);
        Assert.True(message.SentAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void New_WithEmptyTitle_ShouldReturnFailure()
    {
        // Act
        var (result, message) = Message.New(RecipientId.New(), "", "Thanks for signing up.");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(message);
    }

    [Fact]
    public void New_WithEmptyBody_ShouldReturnFailure()
    {
        // Act
        var (result, message) = Message.New(RecipientId.New(), "Welcome!", "");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Null(message);
    }
}
