namespace Lab2.Application.Exceptions;

public class RoomNotAvailableException(string roomName)
    : InvalidOperationException(
        $"Room '{roomName}' is already occupied and cannot be reserved."
    ) { }
