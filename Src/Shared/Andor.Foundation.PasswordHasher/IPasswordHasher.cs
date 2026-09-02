namespace Andor.Foundation.PasswordHasher;

public interface IPasswordHasher
{
    string HashPassword(string password);
}
