using ECommerce.MVC.IServices;
using ECommerce.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.MVC.Controllers;

/// <summary>
/// Admin role management: list all roles, create new role.
/// All actions require the Admin role.
/// </summary>
[Authorize(Roles = "Admin")]
[Route("Admin/Roles")]
public class AdminRolesController : Controller
{
    private readonly IUserAdminService _adminService;

    public AdminRolesController(IUserAdminService adminService)
    {
        _adminService = adminService;
    }

    // ── GET /Admin/Roles ──────────────────────────────────────────────────────

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var vm = await _adminService.GetRoleListAsync();
        return View("~/Views/Admin/Roles/Index.cshtml", vm);
    }

    // ── GET /Admin/Roles/Create ───────────────────────────────────────────────

    [HttpGet("Create")]
    public IActionResult Create() =>
        View("~/Views/Admin/Roles/Create.cshtml", new CreateRoleViewModel());

    // ── POST /Admin/Roles/Create ──────────────────────────────────────────────

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoleViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Admin/Roles/Create.cshtml", model);

        var (success, error) = await _adminService.CreateRoleAsync(model);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, error ?? "Failed to create role.");
            return View("~/Views/Admin/Roles/Create.cshtml", model);
        }

        TempData["Success"] = $"Role '{model.Name}' created successfully.";
        return RedirectToAction(nameof(Index));
    }
}
