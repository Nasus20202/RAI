using Lab2.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Web.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        if (statusCode >= 500)
        {
            _logger.LogError("Server error with status code: {StatusCode}", statusCode);
        }

        var model = statusCode switch
        {
            403 => new ErrorViewModel
            {
                StatusCode = 403,
                Title = "Access Denied",
                ErrorMessage = "You don't have permission to access this resource.",
            },
            404 => new ErrorViewModel
            {
                StatusCode = 404,
                Title = "Page Not Found",
                ErrorMessage = "Sorry, the page you requested could not be found.",
            },
            500 => new ErrorViewModel
            {
                StatusCode = 500,
                Title = "Internal Server Error",
                ErrorMessage = "Sorry, something went wrong on our end.",
            },
            _ => new ErrorViewModel
            {
                StatusCode = statusCode,
                Title = "Error",
                ErrorMessage = "An unexpected error occurred.",
            },
        };

        return View("Error", model);
    }

    [HttpGet]
    [Route("Error")]
    public IActionResult Error()
    {
        _logger.LogError("Unhandled exception occurred");

        var model = new ErrorViewModel
        {
            StatusCode = 500,
            Title = "Error",
            ErrorMessage = "Sorry, something went wrong. Please try again later.",
        };

        return View(model);
    }
}
