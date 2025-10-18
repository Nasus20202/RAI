using Lab2.Application.DTOs;

namespace Lab2.Application.Interfaces.Services;

public interface IUserService
{
    void RegisterUser(RegisterUserDto dto);
    UserDto? AuthenticateUser(LoginDto dto);
    UserDto? GetUserByUsername(string username);
    bool UserExists(string username);
}
