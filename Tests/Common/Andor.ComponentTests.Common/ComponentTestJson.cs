using System.Text.Json;

namespace Andor.ComponentTests.Common;

/// <summary>
/// Shared <see cref="JsonSerializerOptions"/> for component tests: camelCase, case-insensitive,
/// matching what ASP.NET Core's default output formatter produces.
/// </summary>
public static class ComponentTestJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}
