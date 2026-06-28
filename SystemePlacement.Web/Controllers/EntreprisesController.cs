using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Entreprises;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/entreprises")]
[Authorize(Roles = "Employeur")] // Role : Employeur
public class EntreprisesController : ControllerBase
{
    private readonly IEntrepriseService _entrepriseService; // Instance de l'interface service IEntrepriseService

    public EntreprisesController(IEntrepriseService entrepriseService)
    {
        _entrepriseService = entrepriseService;
    }

    // GET /api/entreprises/mon-profil
    [HttpGet("mon-profil")]
    public async Task<ActionResult<EntrepriseResponseDto>> GetMonProfil()
    {
        try
        {
            var entreprise = await _entrepriseService.GetMonProfilAsync();

            if (entreprise == null)
            {
                return NotFound(new { message = "Aucun profil d'entreprise trouvé pour cet employeur." });
            }

            return Ok(entreprise);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST /api/entreprises/mon-profil
    [HttpPost("mon-profil")]
    public async Task<ActionResult<EntrepriseCreateDto>> CreateMonProfil([FromBody] EntrepriseCreateDto dto)
    {
        try
        {
            var entreprise = await _entrepriseService.CreateMonProfilAsync(dto);

            return CreatedAtAction(nameof(GetMonProfil), entreprise);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new {message = ex.Message});
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT /api/entreprises/mon-profil
    [HttpPut("mon-profil")]
    public async Task<IActionResult> UpdateMonProfil([FromBody] EntrepriseUpdateDto dto)
    {
        try
        {
            var updated = await _entrepriseService.UpdateMonProfilAsync(dto);

            if (!updated)
            {
                return NotFound(new { message = "Aucun profil d'entreprise trouvé pour cet employeur." });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

