using Andor.Application.Common;
using Andor.Application.Common.Interfaces;
using Andor.Application.Common.Models.FeatureFlag;
using Microsoft.Extensions.Options;
using Unleash;
using Unleash.ClientFactory;

namespace Andor.Infrastructure.Common.FeatureFlag;

public class FeatureFlagService(
    ICurrentUserService _currentUserService,
    IOptions<ApplicationSettings> appConfig) : IFeatureFlagService
{
    public async Task<bool> IsEnabledAsync(CurrentFeatures feature)
        => await IsEnabledAsync(feature, new Dictionary<string, string>());

    public Task<bool> IsEnabledAsync(CurrentFeatures feature, Dictionary<string, string> attributes)
        => Task.FromResult(GetValue(feature, attributes));

    private bool GetValue(CurrentFeatures feature, Dictionary<string, string> attributes)
    {
        var _unleashConfig = appConfig?.Value?.Unleash ?? new UnleashConfig();

        if (attributes is null)
        {
            attributes = [];
        }

        var context = new UnleashContext
        {
            UserId = _currentUserService.User.Name,
            Properties = attributes
        };

        var settings = new UnleashSettings()
        {
            AppName = _unleashConfig.AppName,
            UnleashApi = new Uri(_unleashConfig.UnleashApi ?? ""),
            CustomHttpHeaders = new Dictionary<string, string>()
                {
                    {"Authorization",_unleashConfig.Authorization ?? "" }
                }
        };

        var unleashFactory = new UnleashClientFactory();

        IUnleash _unleash = unleashFactory.CreateClientAsync(settings, synchronousInitialization: true).GetAwaiter().GetResult();

        return _unleash.IsEnabled(feature.Name, context);
    }
}
