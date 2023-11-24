namespace Family.Budget.Domain.Common.ValuesObjects;
public sealed record DomainErrorModel
{
    public int Code { get; init; }
    public string Message { get; init; }

    public DomainErrorModel(DomainErrorCode code, string message)
    {
        Code = code.Value;
        Message = message;
    }

    public DomainErrorModel(DomainErrorCode code, string message, string innerMessage)
    {
        Code = code.Value;
        Message = message;
    }
}
