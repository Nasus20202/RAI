namespace Lab2.Application.Exceptions;

public class ReservationConflictException(
    string username,
    string roomName,
    DateTime startTime,
    DateTime endTime
)
    : InvalidOperationException(
        $"Cannot create reservation for user '{username}' in room '{roomName}' from {startTime:g} to {endTime:g}. Time conflict with existing reservation."
    ) { }
