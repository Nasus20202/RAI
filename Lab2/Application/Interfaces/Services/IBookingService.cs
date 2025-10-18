using Lab2.Application.DTOs;

namespace Lab2.Application.Interfaces.Services;

public interface IBookingService
{
    BookingDto CreateBooking(string username, CreateBookingDto dto);
    void CancelBooking(string username, CancelBookingDto dto);
    IReadOnlyCollection<BookingDto> GetAllBookings();
    IReadOnlyCollection<BookingDto> GetUserBookings(string username);
    IReadOnlyCollection<BookingDto> GetRoomBookings(string roomName);
    string ExportUserBookingsToICalendar(string username);
}
