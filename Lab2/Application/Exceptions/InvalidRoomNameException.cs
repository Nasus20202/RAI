namespace Lab2.Application.Exceptions;

public class InvalidRoomNameException(string message) : ArgumentException(message, "name") { }
