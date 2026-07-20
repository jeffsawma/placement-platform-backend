using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.DemandesStage;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/demandes-stage")]
[Authorize]
public class DemandesStageController : ControllerBase
{
    private readonly IDemandeStageService _service;

    public DemandesStageController(IDemandeStageService service) => _service = service;

    // US-19 : l'etudiant formule une demande de stage dans un domaine.
    [HttpPost]
    [Authorize(Roles = "Etudiant")]
    public async Task<IActionResult> Creer([FromBody] CreerDemandeStageRequest request)
    {
        var demande = await _service.CreerAsync(request);
        return demande is null
            ? Conflict(new { message = "Demande impossible : profil etudiant introuvable." })
            : Ok(demande);
    }

    // Demandes de stage de l'etudiant connecte.
    [HttpGet("mes")]
    [Authorize(Roles = "Etudiant")]
    public async Task<IActionResult> MesDemandes()
        => Ok(await _service.GetMesDemandesAsync());

    // Reception : les employeurs/admins consultent les demandes d'un domaine.
    [HttpGet("domaine/{idDomaine:int}")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> DemandesParDomaine(int idDomaine)
        => Ok(await _service.GetDemandesParDomaineAsync(idDomaine));
}
