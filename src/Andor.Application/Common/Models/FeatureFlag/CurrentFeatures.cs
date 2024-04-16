using Andor.Domain.Common;

namespace Andor.Application.Common.Models.FeatureFlag;

public sealed record CurrentFeatures : Enumeration<int>
{
    private CurrentFeatures(int key, string name) : base(key, name) { }

    public static readonly CurrentFeatures FeatureFlagToTest = new(0, "FeatureFlagToTest");
}
