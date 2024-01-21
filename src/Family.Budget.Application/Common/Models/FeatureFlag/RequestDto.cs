namespace Family.Budget.Application.Models.FeatureFlag;
using System.Collections.Generic;
public class RequestDto
{
    public string Key { get; set; }
    public CurrentFeatures? FeatureName { get; set; }
    public List<KeyValueDto> KeyValues { get; private set; }

    public RequestDto()
    {
        this.KeyValues = new List<KeyValueDto>();
        this.Key = string.Empty;
    }

    public RequestDto(string key, CurrentFeatures featureName, List<KeyValueDto> keyValues)
    {
        this.KeyValues = new List<KeyValueDto>();
        this.Key = key;
        this.FeatureName = featureName;

        this.KeyValues.AddRange(keyValues);
    }
}
