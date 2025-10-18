namespace Lab2.Application.Exceptions;

public class UserNotFoundException(string username)
    : InvalidOperationException($"User '{username}' not found.") { }
