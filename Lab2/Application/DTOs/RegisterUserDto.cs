namespace Lab2.Application.DTOs;

public record RegisterUserDto(string Username, string Password, string? Role = null);
