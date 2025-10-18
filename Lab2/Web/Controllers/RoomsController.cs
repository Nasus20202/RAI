using Lab2.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Web.Controllers;

[Authorize(Roles = "Admin")]
public class RoomsController(ILogger<RoomsController> logger) : Controller
{
    private readonly ILogger<RoomsController> _logger = logger;

    [HttpGet]
    public IActionResult Manage()
    {
        _logger.LogInformation("Admin viewing room management page");
        return View();
    }
}
