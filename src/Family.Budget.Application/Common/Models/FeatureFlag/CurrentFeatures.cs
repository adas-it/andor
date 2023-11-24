namespace Family.Budget.Application.Models.FeatureFlag;
public class CurrentFeatures
{
    private CurrentFeatures(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public static CurrentFeatures FeatureFlagToTest { get { return new CurrentFeatures("FeatureFlagToTest"); } }
}
