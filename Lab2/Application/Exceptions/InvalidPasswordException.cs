namespace Lab2.Application.Exceptions;

public class InvalidPasswordException(string message) : ArgumentException(message, "password") { }
