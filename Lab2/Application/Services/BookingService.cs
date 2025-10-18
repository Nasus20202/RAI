using System.Text;
using Lab2.Application.DTOs;
using Lab2.Application.Exceptions;
using Lab2.Application.Interfaces.Repositories;
using Lab2.Application.Interfaces.Services;
using Lab2.Domain.Models;

namespace Lab2.Application.Services;

public class BookingService : IBookingService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly object _lock = new();

    public BookingService(
        IReservationRepository reservationRepository,
        IUserRepository userRepository,
        IRoomRepository roomRepository
    )
    {
        _reservationRepository = reservationRepository;
        _userRepository = userRepository;
        _roomRepository = roomRepository;
    }

    public BookingDto CreateBooking(string username, CreateBookingDto dto)
    {
        lock (_lock)
        {
            // Walidacja użytkownika
            var user =
                _userRepository.GetUserByUsername(username)
                ?? throw new UserNotFoundException(username);

            // Walidacja pokoju
            var room =
                _roomRepository.GetRoomByName(dto.RoomName)
                ?? throw new RoomNotFoundException(dto.RoomName);

            // Walidacja czasu rezerwacji
            if (dto.StartTime >= dto.EndTime)
            {
                throw new InvalidReservationTimeException("Start time must be before end time.");
            }

            if (dto.StartTime < DateTime.Now)
            {
                throw new InvalidReservationTimeException("Cannot create reservation in the past.");
            }

            // Walidacja długości rezerwacji
            var duration = dto.EndTime - dto.StartTime;
            if (duration.TotalMinutes < 15)
            {
                throw new InvalidReservationTimeException(
                    "Reservation duration must be at least 15 minutes."
                );
            }

            if (duration.TotalHours > 3)
            {
                throw new InvalidReservationTimeException(
                    "Reservation duration cannot exceed 3 hours."
                );
            }

            // Sprawdzenie czy użytkownik ma już aktywną lub nakładającą się rezerwację
            var userReservations = _reservationRepository.GetReservationsByUser(user);
            var conflictingUserReservation = userReservations.FirstOrDefault(r =>
                (r.IsActive || r.IsUpcoming)
                && r.StartTime < dto.EndTime
                && r.EndTime > dto.StartTime
            );

            if (conflictingUserReservation != null)
            {
                throw new UserHasActiveReservationException(username);
            }

            // Sprawdzenie czy pokój jest dostępny w wybranym czasie
            var roomReservations = _reservationRepository.GetReservationsByRoom(room);
            var conflictingRoomReservation = roomReservations.FirstOrDefault(r =>
                (r.IsActive || r.IsUpcoming)
                && r.StartTime < dto.EndTime
                && r.EndTime > dto.StartTime
            );

            if (conflictingRoomReservation != null)
            {
                throw new RoomNotAvailableException(dto.RoomName);
            }

            // Utworzenie rezerwacji
            var reservation = new Reservation(room, user, dto.StartTime, dto.EndTime);
            _reservationRepository.AddReservation(reservation);

            return BookingDto.FromReservation(reservation);
        }
    }

    public void CancelBooking(string username, CancelBookingDto dto)
    {
        lock (_lock)
        {
            // Walidacja użytkownika
            var user =
                _userRepository.GetUserByUsername(username)
                ?? throw new UserNotFoundException(username);

            // Znalezienie rezerwacji po ID
            var reservation = _reservationRepository.GetReservationById(dto.BookingId);

            if (reservation == null)
            {
                throw new ReservationNotFoundException(
                    $"Booking with ID {dto.BookingId} not found"
                );
            }

            // Sprawdzenie czy użytkownik jest właścicielem rezerwacji
            if (reservation.User.Username != username)
            {
                throw new UnauthorizedAccessException(
                    $"User {username} is not authorized to cancel this booking"
                );
            }

            // Usunięcie rezerwacji z repozytorium
            _reservationRepository.RemoveReservation(reservation);
        }
    }

    public IReadOnlyCollection<BookingDto> GetAllBookings()
    {
        lock (_lock)
        {
            var reservations = _reservationRepository.GetAllReservations();
            return reservations.Select(BookingDto.FromReservation).ToList();
        }
    }

    public IReadOnlyCollection<BookingDto> GetUserBookings(string username)
    {
        lock (_lock)
        {
            var user =
                _userRepository.GetUserByUsername(username)
                ?? throw new UserNotFoundException(username);

            var reservations = _reservationRepository.GetReservationsByUser(user);
            return reservations.Select(BookingDto.FromReservation).ToList();
        }
    }

    public IReadOnlyCollection<BookingDto> GetRoomBookings(string roomName)
    {
        lock (_lock)
        {
            var room =
                _roomRepository.GetRoomByName(roomName)
                ?? throw new RoomNotFoundException(roomName);

            var reservations = _reservationRepository.GetReservationsByRoom(room);
            return reservations.Select(BookingDto.FromReservation).ToList();
        }
    }

    public string ExportUserBookingsToICalendar(string username)
    {
        lock (_lock)
        {
            // Walidacja użytkownika
            var user =
                _userRepository.GetUserByUsername(username)
                ?? throw new UserNotFoundException(username);

            // Pobranie rezerwacji użytkownika
            var bookings = GetUserBookings(username);

            // Filtruj tylko nadchodzące rezerwacje
            var upcomingBookings = bookings.Where(b => b.IsUpcoming).ToList();

            // Generuj zawartość iCalendar
            return GenerateICalendarContent(upcomingBookings, username);
        }
    }

    private static string GenerateICalendarContent(
        IEnumerable<BookingDto> bookings,
        string username
    )
    {
        var sb = new StringBuilder();

        // Nagłówek iCalendar
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//Room Booking System//Bookings//EN");
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:PUBLISH");
        sb.AppendLine($"X-WR-CALNAME:{EscapeText($"My Room Bookings - {username}")}");
        sb.AppendLine("X-WR-TIMEZONE:UTC");

        foreach (var booking in bookings)
        {
            AddEventToCalendar(sb, booking);
        }

        // Stopka iCalendar
        sb.AppendLine("END:VCALENDAR");

        return sb.ToString();
    }

    private static void AddEventToCalendar(StringBuilder sb, BookingDto booking)
    {
        // Generuj unikalny ID dla zdarzenia
        var uid = $"{booking.Id}@roombooking.local";

        // Formatuj daty do formatu iCalendar (yyyyMMddTHHmmssZ)
        var dtStart = booking.StartTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
        var dtEnd = booking.EndTime.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
        var dtStamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");

        // Utwórz podsumowanie i opis zdarzenia
        var summary = $"Room Booking - {booking.RoomName}";
        var description = $"Room booking for {booking.RoomName}\\nBooked by: {booking.Username}";

        // Dodaj zdarzenie
        sb.AppendLine("BEGIN:VEVENT");
        sb.AppendLine($"UID:{uid}");
        sb.AppendLine($"DTSTAMP:{dtStamp}");
        sb.AppendLine($"DTSTART:{dtStart}");
        sb.AppendLine($"DTEND:{dtEnd}");
        sb.AppendLine($"SUMMARY:{EscapeText(summary)}");
        sb.AppendLine($"DESCRIPTION:{EscapeText(description)}");
        sb.AppendLine($"LOCATION:{EscapeText(booking.RoomName)}");
        sb.AppendLine("STATUS:CONFIRMED");
        sb.AppendLine("TRANSP:OPAQUE");
        sb.AppendLine("SEQUENCE:0");
        sb.AppendLine("END:VEVENT");
    }

    private static string EscapeText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        // Escape special characters according to RFC 5545
        return text.Replace("\\", "\\\\")
            .Replace(",", "\\,")
            .Replace(";", "\\;")
            .Replace("\n", "\\n")
            .Replace("\r", "");
    }
}
