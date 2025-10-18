namespace Lab2.Application.Exceptions;

public class RoomNotFoundException(string roomName)
    : InvalidOperationException($"Room '{roomName}' not found.") { }
