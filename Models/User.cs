namespace Middleware.Models;

public class User
{
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;
}
