namespace Andor.Shared.Lookups;

public record Language
{
    public Guid Id { get; init; }
    public string ISO { get; init; }
    public string Name { get; init; }

    private Language(Guid id, string iso, string name)
    {
        Id = id;
        ISO = iso;
        Name = name;
    }

    public static List<Language> GetAll()
    {
        var languages = new List<Language>();

        languages.Add(new Language(Guid.Parse("7ed018e8-f93e-4273-9f23-062763ecf68a"), "en", "English"));
        languages.Add(new Language(Guid.Parse("d47ccae3-428f-496c-b92a-6b821cc63bc7"), "br", "Portugues"));
        languages.Add(new Language(Guid.Parse("ed9713b8-9f51-448a-ad74-8838ac480c5e"), "fr", "French"));

        return languages;
    }

    public static Language GetByISO(string iso)
    {
        var languages = GetAll();
        return languages.FirstOrDefault(lang => lang.ISO.Equals(iso, StringComparison.OrdinalIgnoreCase)) ?? languages.First();
    }

    public static Language GetById(Guid id)
    {
        var languages = GetAll();
        return languages.FirstOrDefault(lang => lang.Id == id) ?? languages.First();
    }

}
