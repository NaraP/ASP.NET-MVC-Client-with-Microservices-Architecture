using ECommerce.AuthApi.IServices;
using ECommerce.AuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.AuthApi.Controllers;

/// <summary>
/// Role catalogue management — list existing roles and create new ones.
/// All endpoints are restricted to <c>Admin</c> users.
///
/// Controller responsibility: HTTP binding, validation, status mapping.
/// Business logic lives in <see cref="IRoleService"/>.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public sealed class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService) =>
        _roleService = roleService;

    // ── GET /api/roles ────────────────────────────────────────────────────────
    /// <summary>List all roles with their assigned user counts.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RoleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll() =>
        Ok(await _roleService.GetAllAsync());

    // ── GET /api/roles/{name} ─────────────────────────────────────────────────
    /// <summary>Return a single role by its name.</summary>
    [HttpGet("{name}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName(string name)
    {
        var role = await _roleService.GetByNameAsync(name);

        return role is null
            ? NotFound(new ProblemDetails { Title = $"Role '{name}' not found.", Status = 404 })
            : Ok(role);
    }

    // ── POST /api/roles ───────────────────────────────────────────────────────
    /// <summary>Create a new role. Name must be unique across the platform.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var (role, error) = await _roleService.CreateAsync(request);

        if (role is null)
            return BadRequest(new ProblemDetails
            {
                Title  = "Role creation failed.",
                Detail = error,
                Status = StatusCodes.Status400BadRequest
            });

        var dto = new RoleDto
        {
            Id          = role.Id,
            Name        = role.Name,
            Description = role.Description,
            IsActive    = role.IsActive,
            UserCount   = 0
        };

        return CreatedAtAction(nameof(GetByName), new { name = role.Name }, dto);
    }
}
