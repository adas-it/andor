using Andor.Application.Dto.Common.ApplicationsErrors.Models;

namespace Andor.Application.Administrations.Configurations.Errors;

public static class ConfigurationApplicationError
{
    public static readonly ErrorModel Validation = new(1000, "");
}
