namespace Andor.Users.WebApi;

public class ApplicationUser
{
    public int Id { get; set; }
    public string UserName { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
}
