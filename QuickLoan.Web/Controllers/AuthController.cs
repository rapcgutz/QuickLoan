using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuickLoan.Web.Interfaces;
using QuickLoan.Web.Models;

namespace QuickLoan.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Dashboard", "Loan");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.LoginAsync(model);

        if (result != null)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Dashboard", "Loan");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Dashboard", "Loan");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var now = GetUserNow(HttpContext);
        int age = CalculateAge(model.DateOfBirth, now);
        if (age < 18)
        {
            ModelState.AddModelError(string.Empty, "User must be at least 18 years old");
            return View(model);
        }

        var result = await _authService.RegisterAsync(model);

        if (result != null && result.Success)
        {
            TempData["SuccessMessage"] = "Registration successful! Welcome to QuickLoan.";
            return RedirectToAction("Dashboard", "Loan");
        }
        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }

    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        TempData["SuccessMessage"] = "You have been logged out successfully.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    public static DateTime GetUserNow(HttpContext context)
    {
        var tzId = context.Session.GetString("UserTimeZone");

        if (string.IsNullOrEmpty(tzId))
            return DateTime.UtcNow;

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
    }

    public static int CalculateAge(DateTime birthDate, DateTime now)
    {
        int age = now.Year - birthDate.Year;

        if (birthDate.Date > now.AddYears(-age))
            age--;

        return age;
    }
}
