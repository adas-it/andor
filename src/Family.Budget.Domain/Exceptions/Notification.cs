namespace Family.Budget.Domain.Exceptions;

using Family.Budget.Domain.Common.ValuesObjects;

public class Notification
{
    public string FieldName { get; private set; }
    public string Message { get; private set; }
    public DomainErrorCode ErrorCode { get; private set; }

    public Notification(string fieldName, string message, DomainErrorCode errorCode)
    {
        this.FieldName = fieldName;
        this.Message = message;
        this.ErrorCode = errorCode;
    }
}

public static class NotificationExtensions
{
    public static string GetMessage(this List<Notification> list)
    {
        var ret = "";

        foreach (Notification notification in list)
        {
            if (ret.Length > 0)
            {
                ret += ";";
            }

            ret += notification.GetMessage();
        }

        return ret;
    }

    public static string GetMessage(this Notification notification)
        => $"{(int)notification.ErrorCode.Value}:{notification.Message}";
}
