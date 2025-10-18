using Lab2.Application.DTOs;
using Lab2.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;
    private readonly ILogger<BookingsController> _logger = logger;

    private string GetCurrentUsername()
    {
        return User.Identity?.Name
            ?? throw new UnauthorizedAccessException("User not authenticated");
    }

    /// <summary>
    /// Create a new booking
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<BookingDto> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var username = GetCurrentUsername();
        _logger.LogInformation(
            "API: User {Username} is creating a booking for room {RoomName} from {StartTime} to {EndTime}",
            username,
            dto.RoomName,
            dto.StartTime,
            dto.EndTime
        );

        var booking = _bookingService.CreateBooking(username, dto);

        _logger.LogInformation(
            "API: Booking {BookingId} created successfully for user {Username}",
            booking.Id,
            username
        );

        return CreatedAtAction(nameof(GetUserBookings), new { }, booking);
    }

    /// <summary>
    /// Cancel a booking by ID
    /// </summary>
    [HttpDelete("{bookingId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult CancelBooking(Guid bookingId)
    {
        var username = GetCurrentUsername();
        _logger.LogInformation(
            "API: User {Username} is canceling booking {BookingId}",
            username,
            bookingId
        );

        _bookingService.CancelBooking(username, new CancelBookingDto(bookingId));

        _logger.LogInformation(
            "API: Booking {BookingId} canceled successfully by user {Username}",
            bookingId,
            username
        );

        return NoContent();
    }

    /// <summary>
    /// Get all bookings for the current user
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<BookingDto>> GetUserBookings()
    {
        var username = GetCurrentUsername();
        _logger.LogInformation("API: Getting all bookings for user {Username}", username);

        var bookings = _bookingService.GetUserBookings(username);
        return Ok(bookings);
    }

    /// <summary>
    /// Get all bookings for a specific day (for calendar view)
    /// </summary>
    [HttpGet("day/{date}")]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<BookingDto>> GetForDay(string date)
    {
        if (!DateTime.TryParse(date, out var parsedDate))
        {
            throw new ArgumentException("Invalid date format. Use YYYY-MM-DD");
        }

        _logger.LogInformation(
            "API: Getting bookings for day {Date}",
            parsedDate.ToShortDateString()
        );

        var allBookings = _bookingService.GetAllBookings();

        // Filter bookings that overlap with the specified day
        var dayStart = parsedDate.Date;
        var dayEnd = parsedDate.Date.AddDays(1);

        var bookingsForDay = allBookings
            .Where(b => b.StartTime < dayEnd && b.EndTime > dayStart)
            .OrderBy(b => b.StartTime)
            .ToList();

        return Ok(bookingsForDay);
    }
}
