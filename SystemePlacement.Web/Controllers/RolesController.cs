using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Roles;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // GET: api/roles
    // Retourne la liste des roles disponibles.
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetAll()
    {
        var roles = await _roleService.GetAllAsync();

        return Ok(roles);
    }
}