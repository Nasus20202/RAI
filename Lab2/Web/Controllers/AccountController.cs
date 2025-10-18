using System.Security.Claims;
using Lab2.Application.DTOs;
using Lab2.Application.Interfaces.Services;
using Lab2.Web.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Web.Controllers;

public class AccountController(IUserService userService, ILogger<AccountController> logger)
    : Controller
{
    private readonly IUserService _userService = userService;
    private readonly ILogger<AccountController> _logger = logger;

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        _logger.LogInformation("Attempting login for user: {Username}", model.Username);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var loginDto = new LoginDto(model.Username!, model.Password!);
        var user = _userService.AuthenticateUser(loginDto);

        if (user == null)
        {
            model.AuthenticationMessage = "Invalid username or password.";
            return View(model);
        }

        // Create claims for the authenticated user
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        return RedirectToAction("Index", "Bookings");
    }

    [HttpGet]
    public ActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        _logger.LogInformation("Attempting registration for user: {Username}", model.Username);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.Password != model.ConfirmPassword)
        {
            model.ErrorMessage = "Passwords do not match.";
            return View(model);
        }

        try
        {
            // Always create new users as regular users (not admins)
            var registerDto = new RegisterUserDto(model.Username!, model.Password!, "User");
            _userService.RegisterUser(registerDto);

            // Auto-login after registration
            var loginDto = new LoginDto(model.Username!, model.Password!);
            var user = _userService.AuthenticateUser(loginDto);

            if (user != null)
            {
                // Create claims for the authenticated user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );
            }

            return RedirectToAction("Index", "Bookings");
        }
        catch (Exception ex)
        {
            model.ErrorMessage = ex.Message;
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation("User logged out: {Username}", User.Identity?.Name);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login");
    }

    [HttpGet]
    [Authorize]
    public IActionResult Profile()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login");
        }

        var user = _userService.GetUserByUsername(username);
        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var model = new ProfileViewModel { Username = user.Username, Role = user.Role };

        return View(model);
    }
}
