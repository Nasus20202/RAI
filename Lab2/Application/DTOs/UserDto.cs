using Lab2.Domain.Models;

namespace Lab2.Application.DTOs;

public record UserDto(string Username, string Role)
{
    public static UserDto FromUser(User user)
    {
        return new UserDto(user.Username, user.UserRole.ToString());
    }
}
