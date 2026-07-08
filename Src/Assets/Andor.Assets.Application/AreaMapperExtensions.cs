using Andor.Assets.Contracts;
using Andor.Assets.Domain.Investments.Areas;

namespace Andor.Assets.Application;

internal static class AreaMapperExtensions
{
    public static AreaOutput? ToAreaOutput(this Area? entity)
    {
        if (entity == null)
            return null;

        return new AreaOutput(entity.Id.ToString(), entity.Name);
    }
}
