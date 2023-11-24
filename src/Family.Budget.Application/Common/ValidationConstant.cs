namespace Family.Budget.Application._Common;
public static class ValidationConstant
{
    public const string InvalidParameter = "{PropertyName} invalid";
    public const string RequiredParameter = "{PropertyName} is a mandatory parameter";
    public const string RequiredField = "{PropertyName} is a mandatory field";
    public const string MaxLengthExceeded = "Number of characters allowed{MaxLength} was exceeded for the field {PropertyName}";
    public const string LengthError = "Number of characters allowed are between {MinLength} and {MaxLength} for the field {PropertyName}";
    public const string WrongEmail = "Wrong Email";
}
