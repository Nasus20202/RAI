namespace Lab2.Domain.Models;

public class User(string Username, string PasswordHash, Role UserRole = Role.User)
{
    public string Username { get; set; } = Username;
    public string PasswordHash { get; set; } = PasswordHash;
    public Role UserRole { get; set; } = UserRole;
}
