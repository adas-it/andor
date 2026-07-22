using Andor.Foundation.Domain.ValuesObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Foundation.Infrastructure;

public static class Converters
{
    public static ValueConverter<Name, string> GetNameConverter()
        => new(id => id!.Value, value => string.IsNullOrEmpty(value) ? Name.Empty : new Name(value));

    public static ValueConverter<Description, string> GetDescriptionConverter()
        => new(id => id!.Value, value => string.IsNullOrEmpty(value) ? Description.Empty : new Description(value));

    public static ValueConverter<Value, string> GetValueConverter()
        => new(id => id!.Value, value => string.IsNullOrEmpty(value) ? Value.Empty : new Value(value));
}
