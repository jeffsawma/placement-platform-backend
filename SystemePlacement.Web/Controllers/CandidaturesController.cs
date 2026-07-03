using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Candidatures;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CandidaturesController : ControllerBase
{
    private readonly ICandidatureService _service;

    public CandidaturesController(ICandidatureService service) => _service = service;

    // Ancienne route : permet de consulter les candidatures d'une offre.
    [HttpGet("offre/{idOffre:int}")]
    public async Task<IActionResult> GetParOffre(int idOffre)
        => Ok(await _service.GetParOffreAsync(idOffre));

    // Nouvelle route Dev3 : plus claire pour l'employeur.
    // GET /api/offres/{idOffre}/candidatures
    [HttpGet("/api/offres/{idOffre:int}/candidatures")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> GetCandidaturesOffre(int idOffre)
    {
        var candidatures = await _service.GetCandidaturesOffreAsync(idOffre);
        return Ok(candidatures);
    }

    // Ancienne route : detail simple d'une candidature.
    [HttpGet("{idCandidature:int}")]
    public async Task<IActionResult> Get(int idCandidature)
    {
        var candidature = await _service.GetAsync(idCandidature);
        return candidature is null ? NotFound() : Ok(candidature);
    }

    // Nouvelle route Dev3 : detail complet d'une candidature.
    [HttpGet("{idCandidature:int}/detail")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> GetDetail(int idCandidature)
    {
        var detail = await _service.GetDetailAsync(idCandidature);
        return detail is null ? NotFound() : Ok(detail);
    }

    // US-11 : liste des candidatures pour un domaine (employeur).
    [HttpGet("domaine/{idDomaine:int}")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> GetCandidaturesParDomaine(int idDomaine)
        => Ok(await _service.GetCandidaturesParDomaineAsync(idDomaine));

    // Candidatures de l'etudiant connecte.
    [HttpGet("mes")]
    public async Task<IActionResult> MesCandidatures()
        => Ok(await _service.GetMesCandidaturesAsync());

    // US-13 : l'etudiant met a jour le message de sa candidature.
    [HttpPut("{idCandidature:int}/mes")]
    public async Task<IActionResult> MettreAJour(int idCandidature, [FromBody] MettreAJourCandidatureRequest request)
        => await _service.MettreAJourAsync(idCandidature, request) ? NoContent() : NotFound();

    // US-13 : l'etudiant retire sa candidature.
    [HttpPost("{idCandidature:int}/retirer")]
    public async Task<IActionResult> Retirer(int idCandidature)
        => await _service.RetirerAsync(idCandidature) ? NoContent() : NotFound();

    // Ancienne route : permet a un etudiant de postuler.
    [HttpPost]
    public async Task<IActionResult> Postuler(PostulerRequest request)
    {
        var candidature = await _service.PostulerAsync(request);
        return candidature is null
            ? Conflict(new { message = "Candidature impossible : CV manquant, deja postule, ou profil etudiant introuvable." })
            : CreatedAtAction(nameof(Get), new { idCandidature = candidature.IdCandidature }, candidature);
    }

    // Ancienne route : changement de statut avec PUT.
    [HttpPut("{idCandidature:int}/statut")]
    public async Task<IActionResult> ChangerStatut(int idCandidature, ChangerStatutRequest request)
        => await _service.ChangerStatutAsync(idCandidature, request) ? NoContent() : NotFound();

    // Nouvelle route Dev3 : changement de statut avec PATCH.
    [HttpPatch("{idCandidature:int}/statut")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> ChangerStatutPatch(
        int idCandidature,
        [FromBody] ChangerStatutCandidatureRequest request)
    {
        var succes = await _service.ChangerStatutAsync(idCandidature, request.Statut);
        return succes ? NoContent() : NotFound();
    }

    // Nouvelle route Dev3 : telecharger le CV ou la lettre de motivation.
    [HttpGet("documents/{idDocument:int}/telecharger")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> TelechargerDocument(int idDocument)
    {
        var result = await _service.TelechargerDocumentAsync(idDocument);
        if (result is null) return NotFound();

        var fichier = result.Value;
        return File(fichier.Contenu, fichier.ContentType, fichier.NomFichier);
    }

    [HttpPost("cv")]
    public async Task<IActionResult> UploadCv(IFormFile fichier)
    {
        if (fichier is null || fichier.Length == 0)
            return BadRequest(new { message = "Aucun fichier fourni." });

        var dossier = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cv");
        Directory.CreateDirectory(dossier);

        var nomFichier = $"{Guid.NewGuid():N}_{Path.GetFileName(fichier.FileName)}";
        var chemin = Path.Combine(dossier, nomFichier);

        using (var stream = new FileStream(chemin, FileMode.Create))
        {
            await fichier.CopyToAsync(stream);
        }

        return Ok(new { url = $"/uploads/cv/{nomFichier}" });
    }
}