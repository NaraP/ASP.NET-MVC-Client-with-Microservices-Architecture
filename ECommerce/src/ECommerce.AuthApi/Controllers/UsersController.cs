using ECommerce.AuthApi.IServices;
using ECommerce.AuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.AuthApi.Controllers;

/// <summary>
/// Admin-level user management: list, view, update, activate/deactivate,
/// and role assignment.
///
/// Controller responsibility: HTTP binding, authorization checks, HTTP status mapping.
/// Business logic lives in <see cref="IUserService"/> and <see cref="IRoleService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public UsersController(IUserService userService, IRoleService roleService)
    {
        _userService = userService;
        _roleService = roleService;
    }

    // ── GET /api/users  [Admin] ───────────────────────────────────────────────
    /// <summary>Return all users with their role assignments. Admin only.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IReadOnlyList<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive = null)
    {
        var users = await _userService.GetAllAsync(isActive);

        var dtos = users.Select(u => new UserDto
        {
            Id          = u.Id,
            FullName    = u.FullName,
            Email       = u.Email,
            PhoneNumber = u.PhoneNumber,
            Roles       = u.UserRoles.Where(ur => ur.Role.IsActive).Select(ur => ur.Role.Name).ToList(),
            IsActive    = u.IsActive,
            CreatedAt   = u.CreatedAt,
            LastLoginAt = u.LastLoginAt
        });

        return Ok(dtos);
    }

    // ── GET /api/users/{id}  [Admin or self] ─────────────────────────────────
    /// <summary>Return a user's profile. Admins can view any user; others only themselves.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        if (!CanAccessUser(id))
            return Forbid();

        var user = await _userService.GetByIdAsync(id);

        if (user is null)
            return NotFound(new ProblemDetails { Title = "User not found.", Status = 404 });

        return Ok(new UserDto
        {
            Id          = user.Id,
            FullName    = user.FullName,
            Email       = user.Email,
            PhoneNumber = user.PhoneNumber,
            Roles       = user.UserRoles.Where(ur => ur.Role.IsActive).Select(ur => ur.Role.Name).ToList(),
            IsActive    = user.IsActive,
            CreatedAt   = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        });
    }

    // ── PUT /api/users/{id}  [Admin or self] ─────────────────────────────────
    /// <summary>
    /// Update name / phone / active flag.
    /// Non-admin users may not change the <c>IsActive</c> flag.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request)
    {
        if (!CanAccessUser(id))
            return Forbid();

        // Non-admin users cannot change their own active state
        if (!IsAdmin)
            request.IsActive = null;

        var (success, error) = await _userService.UpdateAsync(id, request);

        if (!success)
            return BadRequest(new ProblemDetails { Title = "Update failed.", Detail = error, Status = 400 });

        var updated = await _userService.GetByIdAsync(id);
        var roles   = updated!.UserRoles.Where(ur => ur.Role.IsActive).Select(ur => ur.Role.Name).ToList();

        return Ok(new UserDto
        {
            Id          = updated.Id,
            FullName    = updated.FullName,
            Email       = updated.Email,
            PhoneNumber = updated.PhoneNumber,
            Roles       = roles,
            IsActive    = updated.IsActive,
            CreatedAt   = updated.CreatedAt,
            LastLoginAt = updated.LastLoginAt
        });
    }

    // ── PATCH /api/users/{id}/activate  [Admin] ───────────────────────────────
    [HttpPatch("{id}/activate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Activate(string id)
    {
        var (success, error) = await _userService.SetActiveAsync(id, isActive: true);
        return success
            ? Ok(new { message = $"User {id} activated." })
            : BadRequest(new ProblemDetails { Title = "Activation failed.", Detail = error, Status = 400 });
    }

    // ── PATCH /api/users/{id}/deactivate  [Admin] ─────────────────────────────
    [HttpPatch("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Deactivate(string id)
    {
        var (success, error) = await _userService.SetActiveAsync(id, isActive: false);
        return success
            ? Ok(new { message = $"User {id} deactivated." })
            : BadRequest(new ProblemDetails { Title = "Deactivation failed.", Detail = error, Status = 400 });
    }

    // ── POST /api/users/assign-role  [Admin] ──────────────────────────────────
    /// <summary>Assign a named role to a user.</summary>
    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        var assignedBy       = CurrentUserId;
        var (success, error) = await _roleService.AssignRoleAsync(request.UserId, request.RoleName, assignedBy);

        if (!success)
            return BadRequest(new ProblemDetails { Title = "Role assignment failed.", Detail = error, Status = 400 });

        var updatedRoles = await _roleService.GetUserRolesAsync(request.UserId);
        return Ok(new { userId = request.UserId, roles = updatedRoles });
    }

    // ── POST /api/users/remove-role  [Admin] ──────────────────────────────────
    /// <summary>Remove a named role from a user.</summary>
    [HttpPost("remove-role")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleRequest request)
    {
        var (success, error) = await _roleService.RemoveRoleAsync(request.UserId, request.RoleName);

        if (!success)
            return BadRequest(new ProblemDetails { Title = "Role removal failed.", Detail = error, Status = 400 });

        var updatedRoles = await _roleService.GetUserRolesAsync(request.UserId);
        return Ok(new { userId = request.UserId, roles = updatedRoles });
    }

    // ── GET /api/users/{id}/roles  [Admin or self] ────────────────────────────
    [HttpGet("{id}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRoles(string id)
    {
        if (!CanAccessUser(id)) return Forbid();

        var roles = await _roleService.GetUserRolesAsync(id);
        return Ok(new { userId = id, roles });
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    private bool IsAdmin =>
        User.IsInRole("Admin");

    /// <summary>Admins can access any user; non-admins can only access themselves.</summary>
    private bool CanAccessUser(string targetUserId) =>
        IsAdmin || CurrentUserId == targetUserId;
}
