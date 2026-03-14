using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ECommerce.MVC.Controllers;

/// <summary>
/// Handles Login, Register, Logout, and real-time email-availability check.
///
/// Depends only on <see cref="IUserService"/> — no DB, no BCrypt, no JWT
/// generation.  All auth logic lives in ECommerce.AuthApi; this controller
/// just issues the ASP.NET Core cookie and stores the tokens in session.
/// </summary>
public class AccountController : Controller
{
    private const string AccessTokenKey = "JwtToken";
    private const string RefreshTokenKey = "RefreshToken";

    private readonly IUserService _userService;
    private readonly IAuthApiService _authApi;

    public AccountController(IUserService userService, IAuthApiService authApi)
    {
        _userService = userService;
        _authApi = authApi;
    }

    // ───────────────── LOGIN ─────────────────

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userService.ValidateCredentialsAsync(model.Email, model.Password);

        if (user is null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        var authResult = await _authApi.LoginAsync(model.Email, model.Password);

        if (authResult is null)
        {
            ModelState.AddModelError("", "Authentication failed.");
            return View(model);
        }

        await SignInWithCookieAsync(
            user.Id,
            user.Email,
            user.FullName,
            user.Roles ?? new List<string>(),
            model.RememberMe);

        StoreTokensInSession(authResult.AccessToken, authResult.RefreshToken);

        await _userService.UpdateLastLoginAsync(user.Id);

        return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
    }

    // ───────────────── FORGOT PASSWORD ─────────────────

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var exists = await _userService.EmailExistsAsync(model.Email);

        if (!exists)
        {
            ModelState.AddModelError("", "Email not found.");
            return View(model);
        }

        TempData["ResetEmail"] = model.Email;
        TempData.Keep("ResetEmail");

        return RedirectToAction(nameof(ResetPassword));
    }

    // ───────────────── RESET PASSWORD ─────────────────

    [HttpGet]
    public IActionResult ResetPassword()
    {
        var email = TempData["ResetEmail"]?.ToString();

        if (string.IsNullOrEmpty(email))
            return RedirectToAction(nameof(Login));

        TempData.Keep("ResetEmail");

        return View(new ResetPasswordViewModel
        {
            Email = email
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await _userService.ResetPasswordAsync(new Models.ResetPasswordViewModel
        {
            Email = model.Email,
            Password = model.Password
        });

        if (!success)
        {
            ModelState.AddModelError("", "Password reset failed.");
            return View(model);
        }

        TempData["Success"] = "Password reset successful. Please login.";

        return RedirectToAction(nameof(Login));
    }

    // ───────────────── REGISTER ─────────────────

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        var model = new RegisterViewModel
        {
            Roles = new List<SelectListItem>
        {
            new SelectListItem { Text = "Customer", Value = "Customer" },
            new SelectListItem { Text = "Admin", Value = "Admin" },
            new SelectListItem { Text = "Manager", Value = "Manager" },
            new SelectListItem { Text = "Support", Value = "Support" }
        }
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var (success, error, user) = await _userService.RegisterAsync(model);

        if (!success || user is null)
        {
            ModelState.AddModelError("", error ?? "Registration failed.");
            return View(model);
        }

        var authResult = await _authApi.LoginAsync(model.Email, model.Password);

        if (authResult != null)
        {
            await SignInWithCookieAsync(
                user.Id,
                user.Email,
                user.FullName,
                user.Roles ?? new List<string>(),
                false);

            StoreTokensInSession(authResult.AccessToken, authResult.RefreshToken);
        }

        TempData["RegistrationSuccess"] =
            $"Welcome {user.FirstName}! Your account was created.";

        return RedirectToAction("Index", "Home");
    }

    // ───────────────── LOGOUT ─────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = HttpContext.Session.GetString(RefreshTokenKey);

        await _authApi.LogoutAsync(refreshToken ?? "");

        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        HttpContext.Session.Clear();

        return RedirectToAction(nameof(Login));
    }

    // ───────────────── CHECK EMAIL (AJAX) ─────────────────

    [HttpGet]
    public async Task<IActionResult> CheckEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Json(new { available = false });

        var exists = await _userService.EmailExistsAsync(email);

        return Json(new { available = !exists });
    }

    // ───────────────── PRIVATE HELPERS ─────────────────

    private async Task SignInWithCookieAsync(
        string id,
        string email,
        string fullName,
        List<string> roles,
        bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, fullName),
            new Claim("FirstName", fullName.Split(' ')[0])
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            });
    }

    private void StoreTokensInSession(string accessToken, string refreshToken)
    {
        HttpContext.Session.SetString(AccessTokenKey, accessToken);
        HttpContext.Session.SetString(RefreshTokenKey, refreshToken);
    }
}
