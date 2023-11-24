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

public class FeatureFlagService : IFeatureFlagService
{
    private readonly ICurrentUserService _currentUserService;
    public FeatureFlagService(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
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
                AppName = "Default",
                UnleashApi = new Uri("https://adasit-unleash.azurewebsites.net/api"),
                CustomHttpHeaders = new Dictionary<string, string>()
                {
                    {"Authorization","default:development.27578642cb03f21ae8661c9c86e49899ad44a592357660cf19e43b24" }
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
