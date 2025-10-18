namespace Lab2.Infrastructure.Exceptions;

public class RoomAlreadyExistsException(string roomName)
    : InvalidOperationException($"Room with name '{roomName}' already exists.") { }
