using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Users;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrateur")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/users
    // Retourne tous les utilisateurs.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UtilisateurResponseDto>>> GetAll()
    {
        var utilisateurs = await _userService.GetAllAsync();

        return Ok(utilisateurs);
    }

    // GET: api/users/5
    // Retourne un utilisateur par son id.
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

    // POST: api/users
    // Cree un nouvel utilisateur.
    [HttpPost]
    public async Task<ActionResult<UtilisateurResponseDto>> Create(UtilisateurCreateDto request)
    {
        try
        {
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

    // PUT: api/users/5
    // Modifie un utilisateur existant.
    [HttpPut("{idUtilisateur:int}")]
    public async Task<IActionResult> Update(
        int idUtilisateur,
        UtilisateurUpdateDto request)
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

    // PATCH: api/users/5/activer
    // Active un compte utilisateur.
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

    // PATCH: api/users/5/desactiver
    // Desactive un compte utilisateur.
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