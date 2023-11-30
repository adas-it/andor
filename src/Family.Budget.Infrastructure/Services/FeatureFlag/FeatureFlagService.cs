namespace Family.Budget.Infrastructure.Services.FeatureFlag;

using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Family.Budget.Application.Dto.Common.ApplicationsErrors.Models;
using Family.Budget.Application.Dto.Models;
using Family.Budget.Application.Dto.Models.Errors;
using Family.Budget.Application.Models.FeatureFlag;
using Family.Budget.Application.Common.Interfaces;
using Unleash;
using Unleash.ClientFactory;
using Family.Budget.Application.Models.Authorization;
using Family.Budget.Application.Common;
using Microsoft.Extensions.Options;

public class FeatureFlagService : IFeatureFlagService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly Unleash _unleashConfig;

    public FeatureFlagService(ICurrentUserService currentUserService
        ,IOptions<ApplicationSettings> appConfig)
    {
        _currentUserService = currentUserService;
        _unleashConfig = appConfig?.Value?.Unleash ?? new Unleash();
    }

    public async Task<bool> IsEnabledAsync(CurrentFeatures feature)
        => await IsEnabledAsync(feature, new Dictionary<string, string>());

    public async Task<bool> IsEnabledAsync(CurrentFeatures feature, Dictionary<string, string> att)
        => await GetValue(feature, att);

    public async Task<bool> GetValue(CurrentFeatures feature, Dictionary<string, string> att)
    {
        if(att is null)
        {
            att = new Dictionary<string, string>();
        }

        var context = new UnleashContext
        {
            UserId = _currentUserService.User.Name,
            Properties = att
        };

        try
        {
            var settings = new UnleashSettings()
            {
                AppName = _unleashConfig.AppName,
                UnleashApi = new Uri(_unleashConfig?.UnleashApi ?? ""),
                CustomHttpHeaders = new Dictionary<string, string>()
                {
                    {"Authorization",_unleashConfig?.Authorization ?? "" }
                }
            };

            var unleashFactory = new UnleashClientFactory();

            IUnleash _unleash = unleashFactory.CreateClientAsync(settings, synchronousInitialization: true).GetAwaiter().GetResult();

            return _unleash.IsEnabled(feature.Value, context);
        }
        catch (ApiException ex)
        {
            try
            {
                var errorResponseDto = await ex.GetContentAsAsync<DefaultResponse<string>>();

                throw new BusinessException(Errors.UnavailableFeatureFlag(), ex)
                {
                    StatusCode = ex.StatusCode
                };
            }
            catch (JsonReaderException jsonEx)
            {
                throw new BusinessException(Errors.ClientHttp(), jsonEx)
                {
                    StatusCode = ex.StatusCode
                };
            }
        }
    }
}
