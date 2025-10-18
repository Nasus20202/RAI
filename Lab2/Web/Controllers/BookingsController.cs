using System.Text;
using Lab2.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Web.Controllers;

[Authorize]
public class BookingsController(
    IRoomService roomService,
    IBookingService bookingService,
    ILogger<BookingsController> logger
) : Controller
{
    private readonly IRoomService _roomService = roomService;
    private readonly IBookingService _bookingService = bookingService;
    private readonly ILogger<BookingsController> _logger = logger;

    [HttpGet]
    public IActionResult Index()
    {
        var username = User.Identity?.Name;
        _logger.LogInformation("User {Username} is viewing bookings page", username);

        var rooms = _roomService.GetAllRooms();
        ViewBag.Rooms = rooms;

        return View();
    }

    [HttpGet]
    public IActionResult Calendar()
    {
        var username = User.Identity?.Name;
        _logger.LogInformation("User {Username} is viewing calendar page", username);

        return View();
    }

    [HttpGet]
    public IActionResult ExportMyBookings()
    {
        var username =
            User.Identity?.Name ?? throw new UnauthorizedAccessException("User not authenticated");

        _logger.LogInformation("User {Username} is exporting bookings to iCalendar", username);

        var icsContent = _bookingService.ExportUserBookingsToICalendar(username);

        var fileName = $"my-bookings-{DateTime.Now:yyyyMMdd}.ics";
        var bytes = Encoding.UTF8.GetBytes(icsContent);

        return File(bytes, "text/calendar", fileName);
    }
}
