namespace Lab2.Application.DTOs;

public record CreateBookingDto(string RoomName, DateTime StartTime, DateTime EndTime);
