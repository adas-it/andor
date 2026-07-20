namespace Andor.Users.WebApi;

public class ApplicationUser
{
    public int Id { get; set; }
    public string UserName { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;

    /// <summary>
    /// The authorization group this user belongs to (e.g. "Administrador", "Administrador Senior").
    /// Issued into the access token so resource APIs can resolve fine-grained permissions per group
    /// without querying this Identity API on every request.
    /// </summary>
    public string Group { get; set; } = "User";
}
