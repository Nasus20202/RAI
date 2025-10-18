using System.Net;
using System.Text.Json;
using Lab2.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lab2.Web.Filters;

public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger = logger;

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var (statusCode, errorMessage) = exception switch
        {
            UserNotFoundException => (StatusCodes.Status401Unauthorized, exception.Message),
            RoomNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            ReservationNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            InvalidReservationTimeException => (StatusCodes.Status400BadRequest, exception.Message),
            UserHasActiveReservationException => (StatusCodes.Status409Conflict, exception.Message),
            RoomNotAvailableException => (StatusCodes.Status409Conflict, exception.Message),
            InvalidUsernameException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidPasswordException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidRoomNameException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidRoomCapacityException => (StatusCodes.Status400BadRequest, exception.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, exception.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An internal error occurred"),
        };

        if (statusCode >= 500)
        {
            _logger.LogError(
                exception,
                "An unhandled exception occurred: {Message}",
                exception.Message
            );
            return;
        }

        context.HttpContext.Response.ContentType = "application/json";

        var response = new { error = errorMessage };

        context.Result = new ObjectResult(response) { StatusCode = statusCode };

        context.ExceptionHandled = true;
        _logger.LogInformation("Handled exception: {Message}", exception.Message);
    }
}
