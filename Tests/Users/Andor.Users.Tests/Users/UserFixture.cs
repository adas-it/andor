using Andor.Foundation.Domain.ValuesObjects;
using Moq;
using System.Net.Mail;
using Users.Users;
using Users.Users.Errors;
using Users.Users.ValueObjects;

namespace Andor.Users.Domain.Tests.Users;

internal static class UserFixture
{
    public static readonly MailAddress ValidEmail = new("john.doe@example.com");
    public static readonly string ValidFirstName = "John";
    public static readonly string ValidLastName = "Doe";
    public static readonly Guid ValidCurrencyId = Guid.NewGuid();
    public static readonly Guid ValidLanguageId = Guid.NewGuid();

    public static Mock<IUserValidator> CreateDefaultValidator()
    {
        var mock = new Mock<IUserValidator>();
        _ = mock.Setup(v => v.ValidateCreationAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        return mock;
    }

    public static Mock<IUserValidator> CreateEmailInUseValidator()
    {
        var mock = new Mock<IUserValidator>();
        _ = mock.Setup(v => v.ValidateCreationAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new Andor.Foundation.Domain.ValuesObjects.Notification(
                    nameof(User.Email),
                    UserErrorMessages.EmailAlreadyInUse,
                    UserErrorCode.EmailAlreadyInUse)
            ]);
        return mock;
    }

    public static Task<(DomainResult, User?)> CreateValidUserAsync(
        Mock<IUserValidator>? validator = null,
        UserId? userId = null,
        MailAddress? email = null,
        string? firstName = null,
        string? lastName = null,
        Guid? preferredCurrencyId = null,
        Guid? preferredLanguageId = null,
        CancellationToken cancellationToken = default)
        => User.NewAsync(
            userId ?? UserId.New(),
            email ?? ValidEmail,
            firstName ?? ValidFirstName,
            lastName ?? ValidLastName,
            preferredCurrencyId ?? ValidCurrencyId,
            preferredLanguageId ?? ValidLanguageId,
            (validator ?? CreateDefaultValidator()).Object,
            cancellationToken);
}
