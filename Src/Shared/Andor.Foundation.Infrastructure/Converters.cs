using Andor.Foundation.Domain.ValuesObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Foundation.Infrastructure;

public static class Converters
{
    public static ValueConverter<Name, string> GetNameConverter()
        => new(id => id!.Value, value => value);

    public static ValueConverter<Description, string> GetDescriptionConverter()
        => new(id => id!.Value, value => value);

    public static ValueConverter<Value, string> GetValueConverter()
        => new(id => id!.Value, value => value);
}
