namespace Lab2.Application.Exceptions;

public class InvalidRoomCapacityException(string message)
    : ArgumentException(message, "capacity") { }
