namespace Andor.Application.Communications.Services.PartnerHandler;

internal static class TemplateRendering
{
    public static string Render(string text, Dictionary<string, string> values)
    {
        if (values is null || values.Count == 0)
        {
            return text;
        }

        foreach (var (key, value) in values)
        {
            text = text.Replace(key, value);
        }

        return text;
    }
}
