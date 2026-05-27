using Andor.Domain.Common.ValuesObjects;
using Andor.Foundation.Domain.ValuesObjects;

namespace Andor.Foundation.Domain.Tests.ValuesObjects;

public class NotificationTests
{
    [Fact]
    public void ToString_ReturnsExpectedString()
    {
        // Arrange
        var notification = new Notification("FieldName", "Error Message",
            DomainErrorCode.Validation);

        // Act
        var result = notification.ToString();

        // Assert
        Assert.Equal($"{DomainErrorCode.Validation}: Message - Error Message", result);
    }
}
