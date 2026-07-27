using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.DomainesEtudes;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/domaines-etudes")] // Route de base pour les domaines d'études
public class DomainesEtudesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DomainesEtudesController(ApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private bool IsAdministrateur =>
        _currentUserService.Role == "Administrateur";


    // GET /api/domaines-etudes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DomaineEtudeResponseDto>>> GetDomainesEtudes(bool includeInactive = false)
    {
        var query = from domaine in _context.DomainesEtudes.AsNoTracking()
                    join college in _context.Colleges.AsNoTracking()
                        on domaine.IdCollege equals college.IdCollege
                    select new
                    {
                        Domaine = domaine,
                        College = college
                    };

        if (IsAdministrateur)
        {
            if (!_currentUserService.IdCollege.HasValue)
            {
                return Ok(Array.Empty<DomaineEtudeResponseDto>());
            }

            var idCollege = _currentUserService.IdCollege.Value;

            query = query.Where(x => x.Domaine.IdCollege == idCollege);
        }


        if (!includeInactive)
        {
            query = query.Where(x => x.Domaine.Actif);
        }

        var domainesEtudes = await query
            .OrderBy(x => x.Domaine.Nom)
            .Select(x => new DomaineEtudeResponseDto
            {
                IdDomaine = x.Domaine.IdDomaine,
                IdCollege = x.Domaine.IdCollege,
                NomCollege = x.College.Nom,
                Nom = x.Domaine.Nom,
                Code = x.Domaine.Code,
                AccepteStagiaires = x.Domaine.AccepteStagiaires,
                Actif = x.Domaine.Actif
            })
            .ToListAsync();

        return Ok(domainesEtudes); // Retourne la liste de tous les domaines d'études
    }

    // GET /api/domaines-etudes/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<DomaineEtudeResponseDto>> GetDomaineEtudeById(int id)
    {

        var isAdministrateur = IsAdministrateur;
        var idCollegeAdministrateur = 0;

        if (isAdministrateur)
        {
            if (!_currentUserService.IdCollege.HasValue)
            {
                return NotFound(new
                {
                    message = "Domaine d'étude introuvable ou non accessible."
                });
            }

            idCollegeAdministrateur = _currentUserService.IdCollege.Value;
        }

        var domaineEtude = await (
            from domaine in _context.DomainesEtudes.AsNoTracking()
            join college in _context.Colleges.AsNoTracking()
                on domaine.IdCollege equals college.IdCollege
            where domaine.IdDomaine == id
                && (!isAdministrateur ||
                    domaine.IdCollege == idCollegeAdministrateur)

            select new DomaineEtudeResponseDto
            {
                IdDomaine = domaine.IdDomaine,
                IdCollege = domaine.IdCollege,
                NomCollege = college.Nom,
                Nom = domaine.Nom,
                Code = domaine.Code,
                AccepteStagiaires = domaine.AccepteStagiaires,
                Actif = domaine.Actif
            })
            .FirstOrDefaultAsync();

        if (domaineEtude == null)
        {
            return NotFound(new { message = $"Domaine d'étude avec {id} non trouvé." });
        }

        return Ok(domaineEtude);
    }

    // POST /api/domaines-etudes
    [HttpPost]
    [Authorize(Roles = "SuperAdministrateur,Administrateur")]
    public async Task<ActionResult<DomaineEtudeResponseDto>> CreateDomaineEtude([FromBody] DomaineEtudeCreateDto dto)
    {
        var nom = dto.Nom.Trim();
        var code = dto.Code.Trim();

        if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new { message = "Le nom et le code sont obligatoires." });
        }

        var idCollege = dto.IdCollege;

        if (IsAdministrateur)
        {
            if (!_currentUserService.IdCollege.HasValue)
            {
                return BadRequest(new
                {
                    message = "Votre compte administrateur n'est rattaché à aucun collège."
                });
            }

            idCollege = _currentUserService.IdCollege.Value;
        }

        // Vérifier si le collège existe ou non
        var college = await _context.Colleges
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.IdCollege == idCollege &&
                c.Actif);

        if (college == null)
        {
            return BadRequest(new
            {
                message = $"Collège actif avec ID {idCollege} non trouvé."
            });
        }

        // Vérifier si un domaine d'étude avec le meme code existe déjà pour ce collège
        var codeExiste = await _context.DomainesEtudes
            .AnyAsync(d =>
                d.Code == code &&
                d.IdCollege == idCollege);

        if (codeExiste)
        {
            return BadRequest(new { message = $"Un domaine d'étude avec le code '{code}' existe déjà pour ce collège" });
        }


        var domaineEtude = new DomaineEtude
        {
            IdCollege = idCollege, // FK
            Nom = nom,
            Code = code,
            AccepteStagiaires = dto.AccepteStagiaires,
            Actif = dto.Actif
        };

        _context.DomainesEtudes.Add(domaineEtude);
        await _context.SaveChangesAsync();

        var response = new DomaineEtudeResponseDto
        {
            IdDomaine = domaineEtude.IdDomaine,
            IdCollege = domaineEtude.IdCollege,
            NomCollege = college.Nom,
            Nom = domaineEtude.Nom,
            Code = domaineEtude.Code,
            AccepteStagiaires = domaineEtude.AccepteStagiaires,
            Actif = domaineEtude.Actif
        };

        return CreatedAtAction(nameof(GetDomaineEtudeById), new { id = domaineEtude.IdDomaine }, response);
    }

    // PUT /api/domaines-etudes
    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdministrateur,Administrateur")]
    public async Task<IActionResult> UpdateDomaineEtude(int id, [FromBody] DomaineEtudeUpdateDto dto)
    {
        var nom = dto.Nom.Trim();
        var code = dto.Code.Trim();

        if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new { message = "Le nom et le code sont obligatoires." });
        }

        var domaineQuery = _context.DomainesEtudes.AsQueryable();

        var idCollege = dto.IdCollege;

        if (IsAdministrateur)
        {
            if (!_currentUserService.IdCollege.HasValue)
            {
                return BadRequest(new
                {
                    message = "Votre compte administrateur n'est rattaché à aucun collège."
                });
            }

            idCollege = _currentUserService.IdCollege.Value;

            domaineQuery = domaineQuery.Where(
                d => d.IdCollege == idCollege);
        }

        var existingDomaineEtude = await domaineQuery
            .FirstOrDefaultAsync(d => d.IdDomaine == id);

        if (existingDomaineEtude == null)
        {
            return NotFound(new { message = $"Domaine d'étude avec ID {id} non trouvé." });
        }


        // Vérifier si le collège existe ou non
        var collegeExiste = await _context.Colleges
            .AnyAsync(c =>
                c.IdCollege == idCollege &&
                c.Actif);

        if (!collegeExiste)
        {
            return BadRequest(new
            {
                message = $"Collège actif avec ID {idCollege} non trouvé."
            });
        }

        // Vérifier si un autre domaine d'étude avec le meme code existe déjà pour ce collège
        var codeExiste = await _context.DomainesEtudes
            .AnyAsync(d =>
                d.Code == code &&
                d.IdCollege == idCollege &&
                d.IdDomaine != id);

        if (codeExiste)
        {
            return BadRequest(new { message = $"Un autre domaine d'étude avec le code '{code}' existe déjà pour ce collège." });
        }

        existingDomaineEtude.IdCollege = idCollege;
        existingDomaineEtude.Nom = nom;
        existingDomaineEtude.Code = code;
        existingDomaineEtude.AccepteStagiaires = dto.AccepteStagiaires;
        existingDomaineEtude.Actif = dto.Actif;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /api/domaines-etudes/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdministrateur,Administrateur")]
    public async Task<IActionResult> DeleteDomaineEtude(int id)
    {
        var domaineQuery = _context.DomainesEtudes.AsQueryable();

        if (IsAdministrateur)
        {
            if (!_currentUserService.IdCollege.HasValue)
            {
                return BadRequest(new
                {
                    message = "Votre compte administrateur n'est rattaché à aucun collège."
                });
            }

            var idCollege = _currentUserService.IdCollege.Value;

            domaineQuery = domaineQuery.Where(
                d => d.IdCollege == idCollege);
        }

        var domaineEtude = await domaineQuery
            .FirstOrDefaultAsync(d => d.IdDomaine == id);

        if (domaineEtude == null)
        {
            return NotFound(new
            {
                message = $"Domaine d'étude avec ID {id} non trouvé ou non accessible."
            });
        }

        domaineEtude.Actif = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

// Ce controller gerera les routes liées aux domaines d'études, comme :
// - GET /api/domaines-etudes : Récupérer la liste de tous les domaines d'études
// - GET /api/domaines-etudes/{id} : Récupérer les détails d'un domaine d'étude spécifique
// - POST /api/domaines-etudes : Créer un nouveau domaine d'étude
// - PUT /api/domaines-etudes/{id} : Mettre à jour les informations d'un domaine d'étude existant
// - DELETE /api/domaines-etudes/{id} : Désactiver un domaine d'étude
