using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.MVC.Controllers;

/// <summary>
/// Admin user management: list, create, edit, details, activate/deactivate,
/// assign role, remove role. All actions require the Admin role.
/// Depends only on <see cref="IUserAdminService"/> — no HTTP clients here.
/// </summary>
[Authorize(Roles = "Admin")]
[Route("Admin/Users")]
public class AdminUsersController : Controller
{
    private readonly IUserAdminService _adminService;

    public AdminUsersController(IUserAdminService adminService) =>
        _adminService = adminService;

    // ── GET /Admin/Users ──────────────────────────────────────────────────────

    [HttpGet("")]
    public async Task<IActionResult> Index(
        string? search      = null,
        string? filterRole  = null,
        bool?   filterActive = null)
    {
        var vm = await _adminService.GetUserListAsync(search, filterRole, filterActive);
        return View("~/Views/Admin/Users/Index.cshtml", vm);
    }

    // ── GET /Admin/Users/Details/{id} ─────────────────────────────────────────

    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        var vm = await _adminService.GetUserDetailsAsync(id);
        if (vm is null) return NotFound();
        return View("~/Views/Admin/Users/Details.cshtml", vm);
    }

    // ── GET /Admin/Users/Create ───────────────────────────────────────────────

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateUserViewModel
        {
            AvailableRoles = await _adminService.GetAvailableRolesAsync()
        };

        return View("Views/Admin/Users/Create.cshtml", vm);
    }

    // ── POST /Admin/Users/Create ──────────────────────────────────────────────

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Models.CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableRoles = await _adminService.GetAvailableRolesAsync();
            return View("~/Views/Admin/Users/Create.cshtml", model);
        }

        var (success, error) = await _adminService.CreateUserAsync(model);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, error ?? "Failed to create user.");
            model.AvailableRoles = await _adminService.GetAvailableRolesAsync();
            return View("~/Views/Admin/Users/Create.cshtml", model);
        }

        TempData["Success"] = $"User {model.FirstName} {model.LastName} created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── GET /Admin/Users/Edit/{id} ────────────────────────────────────────────

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var vm = await _adminService.GetUserForEditAsync(id);
        if (vm is null) return NotFound();
        return View("~/Views/Admin/Users/Edit.cshtml", vm);
    }

    // ── POST /Admin/Users/Edit/{id} ───────────────────────────────────────────

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, EditUserViewModel model)
    {
        // Remove role-related keys from ModelState — they're handled separately
        ModelState.Remove(nameof(model.RoleToAssign));

        if (!ModelState.IsValid)
        {
            model.UserId         = id;
            model.AvailableRoles = await _adminService.GetAvailableRolesAsync();
            return View("~/Views/Admin/Users/Edit.cshtml", model);
        }

        var (success, error) = await _adminService.UpdateUserAsync(id, model);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, error ?? "Failed to update user.");
            model.UserId         = id;
            model.AvailableRoles = await _adminService.GetAvailableRolesAsync();
            return View("~/Views/Admin/Users/Edit.cshtml", model);
        }

        TempData["Success"] = "User updated successfully.";
        return RedirectToAction(nameof(Edit), new { id });
    }

    // ── POST /Admin/Users/Activate ────────────────────────────────────────────

    [HttpPost("Activate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(string userId, string? returnUrl = null)
    {
        var (success, error) = await _adminService.ToggleUserActiveAsync(userId, activate: true);
        TempData[success ? "Success" : "Error"] = success ? "User activated." : error;
        return RedirectToUrl(returnUrl);
    }

    // ── POST /Admin/Users/Deactivate ──────────────────────────────────────────

    [HttpPost("Deactivate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(string userId, string? returnUrl = null)
    {
        var (success, error) = await _adminService.ToggleUserActiveAsync(userId, activate: false);
        TempData[success ? "Success" : "Error"] = success ? "User deactivated." : error;
        return RedirectToUrl(returnUrl);
    }

    // ── POST /Admin/Users/AssignRole ──────────────────────────────────────────

    [HttpPost("AssignRole")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(RoleActionViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid role assignment request.";
            return RedirectToUrl(returnUrl);
        }

        var (success, error) = await _adminService.AssignRoleAsync(model.UserId, model.RoleName);
        TempData[success ? "Success" : "Error"] =
            success ? $"Role '{model.RoleName}' assigned." : error;

        return RedirectToUrl(returnUrl ?? Url.Action(nameof(Edit), new { id = model.UserId }));
    }

    // ── POST /Admin/Users/RemoveRole ──────────────────────────────────────────

    [HttpPost("RemoveRole")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveRole(RoleActionViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid role removal request.";
            return RedirectToUrl(returnUrl);
        }

        var (success, error) = await _adminService.RemoveRoleAsync(model.UserId, model.RoleName);
        TempData[success ? "Success" : "Error"] =
            success ? $"Role '{model.RoleName}' removed." : error;

        return RedirectToUrl(returnUrl ?? Url.Action(nameof(Edit), new { id = model.UserId }));
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private IActionResult RedirectToUrl(string? url) =>
        !string.IsNullOrEmpty(url) && Url.IsLocalUrl(url)
            ? Redirect(url)
            : RedirectToAction(nameof(Index));
}
