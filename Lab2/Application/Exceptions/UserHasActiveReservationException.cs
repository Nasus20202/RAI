namespace Lab2.Application.Exceptions;

public class UserHasActiveReservationException(string username)
    : InvalidOperationException($"User '{username}' already has an active reservation.") { }
