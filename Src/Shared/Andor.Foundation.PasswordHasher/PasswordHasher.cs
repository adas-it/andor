namespace Andor.Foundation.PasswordHasher;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return new Microsoft.AspNetCore.Identity.PasswordHasher<object>().HashPassword(null!, password);
    }
}
