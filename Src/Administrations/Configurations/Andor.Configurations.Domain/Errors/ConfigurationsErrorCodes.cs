using Andor.Domain.Common.ValuesObjects;

namespace Andor.Configurations.Domain.Errors;

public record ConfigurationsErrorCodes
{
    public static readonly DomainErrorCode ActionNotAllowed = DomainErrorCode.New(2_000);
    public static readonly DomainErrorCode ConfigurationNotFound = DomainErrorCode.New(2_001);

    // Deactivation errors
    public static readonly DomainErrorCode ErrorOnDeactivationConfigurationNotAllowedAwaiting = DomainErrorCode.New(2_002);
    public static readonly DomainErrorCode ErrorOnDeactivationConfigurationNotAllowedExpired = DomainErrorCode.New(2_003);
    public static readonly DomainErrorCode ErrorOnDeactivationConfigurationNotAllowedDeleted = DomainErrorCode.New(2_004);

    // Delete errors
    public static readonly DomainErrorCode ErrorOnDeleteConfiguration = DomainErrorCode.New(2_005);
    public static readonly DomainErrorCode ErrorOnDeleteConfigurationNotAllowedDeleteActive = DomainErrorCode.New(2_006);
    public static readonly DomainErrorCode ErrorOnDeleteConfigurationNotAllowedDeleteExpired = DomainErrorCode.New(2_007);

    // Update errors
    public static readonly DomainErrorCode OnlyDescriptionAllowedToChange = DomainErrorCode.New(2_008);
    public static readonly DomainErrorCode ErrorOnChangeName = DomainErrorCode.New(2_009);

    // Info codes
    public static readonly DomainErrorCode SetExpireDateToToday = DomainErrorCode.New(2_010);
    public static readonly DomainErrorCode ThereWillCurrentConfigurationStartDate = DomainErrorCode.New(2_011);
    public static readonly DomainErrorCode ThereWillCurrentConfigurationEndDate = DomainErrorCode.New(2_012);
    public static readonly DomainErrorCode SkippedValidations = DomainErrorCode.New(2_013);
}
