namespace Lab2.Infrastructure.Exceptions;

public class UserAlreadyExistsException(string username)
    : InvalidOperationException($"User with username '{username}' already exists.") { }
