using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Users;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdministrateur,Administrateur")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UtilisateurResponseDto>>> GetAll()
    {
        // SuperAdmin verra tout.
        // Admin verra seulement son college quand le filtre sera ajoute dans UserService.
        return Ok(await _userService.GetAllAsync());
    }

    [HttpGet("{idUtilisateur:int}")]
    public async Task<ActionResult<UtilisateurResponseDto>> GetById(int idUtilisateur)
    {
        var utilisateur = await _userService.GetByIdAsync(idUtilisateur);

        if (utilisateur == null)
        {
            return NotFound(new { message = "Utilisateur introuvable." });
        }

        return Ok(utilisateur);
    }

    [HttpPost]
    public async Task<ActionResult<UtilisateurResponseDto>> Create(UtilisateurCreateDto request)
    {
        try
        {
            // La logique du college sera validee dans UserService.
            var utilisateur = await _userService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { idUtilisateur = utilisateur.IdUtilisateur },
                utilisateur);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{idUtilisateur:int}")]
    public async Task<IActionResult> Update(int idUtilisateur, UtilisateurUpdateDto request)
    {
        try
        {
            var updated = await _userService.UpdateAsync(idUtilisateur, request);

            if (!updated)
            {
                return NotFound(new { message = "Utilisateur introuvable." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{idUtilisateur:int}/activer")]
    public async Task<IActionResult> Activer(int idUtilisateur)
    {
        var updated = await _userService.SetActifAsync(idUtilisateur, true);

        if (!updated)
        {
            return NotFound(new { message = "Utilisateur introuvable." });
        }

        return NoContent();
    }

    [HttpPatch("{idUtilisateur:int}/desactiver")]
    public async Task<IActionResult> Desactiver(int idUtilisateur)
    {
        var updated = await _userService.SetActifAsync(idUtilisateur, false);

        if (!updated)
        {
            return NotFound(new { message = "Utilisateur introuvable." });
        }

        return NoContent();
    }
}
