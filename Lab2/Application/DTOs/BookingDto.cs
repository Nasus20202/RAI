using Lab2.Domain.Models;

namespace Lab2.Application.DTOs;

public record BookingDto(
    Guid Id,
    string RoomName,
    string Username,
    DateTime StartTime,
    DateTime EndTime,
    bool IsActive,
    bool IsUpcoming
)
{
    public static BookingDto FromReservation(Reservation reservation)
    {
        return new BookingDto(
            reservation.Id,
            reservation.ReservedRoom.Name,
            reservation.User.Username,
            reservation.StartTime,
            reservation.EndTime,
            reservation.IsActive,
            reservation.IsUpcoming
        );
    }
}
