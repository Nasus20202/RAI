using Lab2.Application.DTOs;
using Lab2.Application.Exceptions;
using Lab2.Application.Interfaces.Repositories;
using Lab2.Application.Interfaces.Services;
using Lab2.Domain.Models;

namespace Lab2.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void RegisterUser(RegisterUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username))
        {
            throw new InvalidUsernameException("Username cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new InvalidPasswordException("Password cannot be empty.");
        }

        if (dto.Username.Length < 3)
        {
            throw new InvalidUsernameException("Username must be at least 3 characters long.");
        }

        if (dto.Password.Length < 6)
        {
            throw new InvalidPasswordException("Password must be at least 6 characters long.");
        }

        var role = string.IsNullOrWhiteSpace(dto.Role)
            ? Role.User
            : Enum.Parse<Role>(dto.Role, ignoreCase: true);

        var passwordHash = HashPassword(dto.Password);
        var user = new User(dto.Username, passwordHash, role);
        _userRepository.AddUser(user);
    }

    public UserDto? AuthenticateUser(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return null;
        }

        var user = _userRepository.GetUserByUsername(dto.Username);
        if (user == null)
        {
            return null;
        }

        var passwordHash = HashPassword(dto.Password);
        return user.PasswordHash == passwordHash ? UserDto.FromUser(user) : null;
    }

    public UserDto? GetUserByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new InvalidUsernameException("Username cannot be empty.");
        }

        var user = _userRepository.GetUserByUsername(username);
        return user != null ? UserDto.FromUser(user) : null;
    }

    public bool UserExists(string username)
    {
        return _userRepository.GetUserByUsername(username) != null;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
