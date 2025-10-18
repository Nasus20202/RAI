namespace Lab2.Application.Exceptions;

public class InvalidReservationTimeException(string message) : ArgumentException(message) { }
