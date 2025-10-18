namespace Lab2.Application.Exceptions;

public class ReservationNotFoundException : InvalidOperationException
{
    public ReservationNotFoundException(string username, string roomName)
        : base($"No active reservation found for user '{username}' in room '{roomName}'.") { }

    public ReservationNotFoundException(string message)
        : base(message) { }
}
