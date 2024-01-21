namespace Family.Budget.Application.@Common.Interfaces;

using Family.Budget.Application.Models.FeatureFlag;
using System.Collections.Generic;
using System.Threading.Tasks;
public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(CurrentFeatures feature);
    Task<bool> IsEnabledAsync(CurrentFeatures feature, Dictionary<string, string> att);
}
