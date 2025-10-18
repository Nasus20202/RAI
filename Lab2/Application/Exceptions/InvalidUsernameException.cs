namespace Lab2.Application.Exceptions;

public class InvalidUsernameException(string message) : ArgumentException(message, "username") { }
