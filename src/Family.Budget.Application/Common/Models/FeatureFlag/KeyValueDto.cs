namespace Family.Budget.Application.Models.FeatureFlag;
public class KeyValueDto
{
    public string Key { get; set; }
    public string Value { get; set; }

    public KeyValueDto()
    {
        Key = string.Empty;
        Value = string.Empty;
    }

    public KeyValueDto(string key, string value)
    {
        this.Key = key;
        this.Value = value;
    }
}
