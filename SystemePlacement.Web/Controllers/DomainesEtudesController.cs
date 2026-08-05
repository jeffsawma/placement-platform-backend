using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.DomainesEtudes;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/domaines-etudes")]
[Authorize]
public class DomainesEtudesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DomainesEtudesController(
        ApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    private bool IsAdministrateur =>
        _currentUserService.Role == "Administrateur";

    private bool IsSuperAdministrateur =>
        _currentUserService.Role == "SuperAdministrateur";

    // GET /api/domaines-etudes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DomaineEtudeResponseDto>>> GetDomainesEtudes(
        bool includeInactive = false)
    {
        var query = _context.DomainesEtudes
            .AsNoTracking()
            .Include(d => d.CollegeDomaines)
                .ThenInclude(cd => cd.College)
            .AsQueryable();

        // Un administrateur voit seulement les domaines lies a son college.
        if (IsAdministrateur)
        {
            if (!_currentUserService.IdCollege.HasValue)
            {
                return Ok(Array.Empty<DomaineEtudeResponseDto>());
            }

            var idCollege = _currentUserService.IdCollege.Value;

            query = query.Where(d =>
                d.CollegeDomaines.Any(cd =>
                    cd.IdCollege == idCollege &&
                    (includeInactive || cd.Actif)));
        }

        if (!includeInactive)
        {
            query = query.Where(d => d.Actif);
        }

        var domaines = await query
            .OrderBy(d => d.Nom)
            .ToListAsync();

        return Ok(domaines.Select(MapDomaineResponse));
    }

    // GET /api/domaines-etudes/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DomaineEtudeResponseDto>> GetDomaineEtudeById(int id)
    {
        var query = _context.DomainesEtudes
            .AsNoTracking()
            .Include(d => d.CollegeDomaines)
                .ThenInclude(cd => cd.College)
            .AsQueryable();

        if (IsAdministrateur)
        {
            if (!_currentUserService.IdCollege.HasValue)
            {
                return NotFound(new
                {
                    message = "Domaine d'etude introuvable ou non accessible."
                });
            }

            var idCollege = _currentUserService.IdCollege.Value;

            query = query.Where(d =>
                d.CollegeDomaines.Any(cd => cd.IdCollege == idCollege));
        }

        var domaine = await query.FirstOrDefaultAsync(d => d.IdDomaine == id);

        if (domaine == null)
        {
            return NotFound(new
            {
                message = $"Domaine d'etude avec ID {id} non trouve."
            });
        }

        return Ok(MapDomaineResponse(domaine));
    }

    // POST /api/domaines-etudes
    [HttpPost]
    [Authorize(Roles = "SuperAdministrateur,Administrateur")]
    public async Task<ActionResult<DomaineEtudeResponseDto>> CreateDomaineEtude(
        [FromBody] DomaineEtudeCreateDto dto)
    {
        var nom = dto.Nom.Trim();
        var code = dto.Code.Trim();

        if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new { message = "Le nom et le code sont obligatoires." });
        }

        var domaineExistant = await _context.DomainesEtudes
            .Include(d => d.CollegeDomaines)
                .ThenInclude(cd => cd.College)
            .FirstOrDefaultAsync(d =>
                d.Nom == nom ||
                d.Code == code);

        if (IsAdministrateur)
        {
            return await CreateOuLierDomainePourAdministrateur(dto, domaineExistant, nom, code);
        }

        if (IsSuperAdministrateur)
        {
            return await CreateOuLierDomainePourSuperAdministrateur(dto, domaineExistant, nom, code);
        }

        return Forbid();
    }

    // PUT /api/domaines-etudes/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdministrateur")]
    public async Task<IActionResult> UpdateDomaineEtude(
        int id,
        [FromBody] DomaineEtudeUpdateDto dto)
    {
        var nom = dto.Nom.Trim();
        var code = dto.Code.Trim();

        if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new { message = "Le nom et le code sont obligatoires." });
        }

        var domaine = await _context.DomainesEtudes
            .FirstOrDefaultAsync(d => d.IdDomaine == id);

        if (domaine == null)
        {
            return NotFound(new { message = $"Domaine d'etude avec ID {id} non trouve." });
        }

        var doublon = await _context.DomainesEtudes
            .AnyAsync(d =>
                d.IdDomaine != id &&
                (d.Nom == nom || d.Code == code));

        if (doublon)
        {
            return BadRequest(new
            {
                message = "Un autre domaine avec le même nom ou le même code existe déjà."
            });
        }

        domaine.Nom = nom;
        domaine.Code = code;
        domaine.Actif = dto.Actif;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST /api/domaines-etudes/{idDomaine}/colleges/{idCollege}
    [HttpPost("{idDomaine:int}/colleges/{idCollege:int}")]
    [Authorize(Roles = "SuperAdministrateur")]
    public async Task<IActionResult> AjouterCollegeAuDomaine(
        int idDomaine,
        int idCollege,
        [FromBody] CollegeDomaineUpdateDto dto)
    {
        var domaineExiste = await _context.DomainesEtudes
            .AnyAsync(d => d.IdDomaine == idDomaine);

        if (!domaineExiste)
        {
            return NotFound(new { message = "Domaine introuvable." });
        }

        var collegeExiste = await _context.Colleges
            .AnyAsync(c => c.IdCollege == idCollege && c.Actif);

        if (!collegeExiste)
        {
            return NotFound(new { message = "College actif introuvable." });
        }

        var lienExiste = await _context.CollegeDomaines
            .AnyAsync(cd => cd.IdDomaine == idDomaine && cd.IdCollege == idCollege);

        if (lienExiste)
        {
            return BadRequest(new
            {
                message = "Ce collège est déjà lié à ce domaine."
            });
        }

        var lien = new CollegeDomaine
        {
            IdDomaine = idDomaine,
            IdCollege = idCollege,
            AccepteStagiaires = dto.AccepteStagiaires,
            Actif = dto.Actif
        };

        _context.CollegeDomaines.Add(lien);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT /api/domaines-etudes/{idDomaine}/colleges/{idCollege}
    [HttpPut("{idDomaine:int}/colleges/{idCollege:int}")]
    [Authorize(Roles = "SuperAdministrateur,Administrateur")]
    public async Task<IActionResult> ModifierLienCollegeDomaine(
        int idDomaine,
        int idCollege,
        [FromBody] CollegeDomaineUpdateDto dto)
    {
        if (IsAdministrateur)
        {
            if (!_currentUserService.IdCollege.HasValue)
            {
                return BadRequest(new
                {
                    message = "Votre compte administrateur n'est rattache a aucun college."
                });
            }

            // Un admin peut seulement modifier le lien de son propre college.
            idCollege = _currentUserService.IdCollege.Value;
        }

        var lien = await _context.CollegeDomaines
            .FirstOrDefaultAsync(cd =>
                cd.IdDomaine == idDomaine &&
                cd.IdCollege == idCollege);

        if (lien == null)
        {
            return NotFound(new
            {
                message = "Lien entre ce college et ce domaine introuvable."
            });
        }

        lien.AccepteStagiaires = dto.AccepteStagiaires;
        lien.Actif = dto.Actif;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /api/domaines-etudes/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdministrateur,Administrateur")]
    public async Task<IActionResult> DeleteDomaineEtude(int id)
    {
        if (IsAdministrateur)
        {
            if (!_currentUserService.IdCollege.HasValue)
            {
                return BadRequest(new
                {
                    message = "Votre compte administrateur n'est rattache a aucun college."
                });
            }

            var idCollege = _currentUserService.IdCollege.Value;

            var lien = await _context.CollegeDomaines
                .FirstOrDefaultAsync(cd =>
                    cd.IdDomaine == id &&
                    cd.IdCollege == idCollege);

            if (lien == null)
            {
                return NotFound(new
                {
                    message = "Domaine introuvable ou non accessible pour votre college."
                });
            }

            // L'admin désactive seulement le domaine pour son collège.
            lien.Actif = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        var domaine = await _context.DomainesEtudes
            .FirstOrDefaultAsync(d => d.IdDomaine == id);

        if (domaine == null)
        {
            return NotFound(new
            {
                message = $"Domaine d'etude avec ID {id} non trouve."
            });
        }

        // Le superadmin désactive le domaine global.
        domaine.Actif = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<ActionResult<DomaineEtudeResponseDto>> CreateOuLierDomainePourAdministrateur(
        DomaineEtudeCreateDto dto,
        DomaineEtude? domaineExistant,
        string nom,
        string code)
    {
        if (!_currentUserService.IdCollege.HasValue)
        {
            return BadRequest(new
            {
                message = "Votre compte administrateur n'est rattache a aucun college."
            });
        }

        var idCollege = _currentUserService.IdCollege.Value;

        var collegeExiste = await _context.Colleges
            .AnyAsync(c => c.IdCollege == idCollege && c.Actif);

        if (!collegeExiste)
        {
            return BadRequest(new
            {
                message = "Votre college est introuvable ou inactif."
            });
        }

        var domaine = domaineExistant;
        if (domaine != null && domaine.Nom != nom)
        {
            return BadRequest(new
            {
                message = "Un domaine avec ce code existe déjà avec un autre nom."
            });
        }

        if (domaine != null && domaine.Code != code)
        {
            return BadRequest(new
            {
                message = "Un domaine avec ce nom existe déjà avec un autre code."
            });
        }

        if (domaine == null)
        {
            domaine = new DomaineEtude
            {
                Nom = nom,
                Code = code,
                Actif = dto.Actif
            };

            _context.DomainesEtudes.Add(domaine);
            await _context.SaveChangesAsync();
        }

        var lienExiste = await _context.CollegeDomaines
            .AnyAsync(cd =>
                cd.IdDomaine == domaine.IdDomaine &&
                cd.IdCollege == idCollege);

        if (lienExiste)
        {
            return BadRequest(new
            {
                message = "Ce domaine est déjà lié à votre collège."
            });
        }

        _context.CollegeDomaines.Add(new CollegeDomaine
        {
            IdDomaine = domaine.IdDomaine,
            IdCollege = idCollege,
            AccepteStagiaires = dto.Colleges.FirstOrDefault()?.AccepteStagiaires ?? true,
            Actif = true
        });

        await _context.SaveChangesAsync();

        var response = await GetDomaineCompletAsync(domaine.IdDomaine);
        return CreatedAtAction(nameof(GetDomaineEtudeById), new { id = domaine.IdDomaine }, response);
    }

    private async Task<ActionResult<DomaineEtudeResponseDto>> CreateOuLierDomainePourSuperAdministrateur(
        DomaineEtudeCreateDto dto,
        DomaineEtude? domaineExistant,
        string nom,
        string code)
    {
        var domaine = domaineExistant;

        if (domaine != null && domaine.Nom != nom)
        {
            return BadRequest(new
            {
                message = "Un domaine avec ce code existe déjà avec un autre nom."
            });
        }

        if (domaine != null && domaine.Code != code)
        {
            return BadRequest(new
            {
                message = "Un domaine avec ce nom existe déjà avec un autre code."
            });
        }

        if (domaine == null)
        {
            domaine = new DomaineEtude
            {
                Nom = nom,
                Code = code,
                Actif = dto.Actif
            };

            _context.DomainesEtudes.Add(domaine);
            await _context.SaveChangesAsync();
        }

        foreach (var collegeDto in dto.Colleges)
        {
            var collegeExiste = await _context.Colleges
                .AnyAsync(c => c.IdCollege == collegeDto.IdCollege && c.Actif);

            if (!collegeExiste)
            {
                return BadRequest(new
                {
                    message = $"College actif avec ID {collegeDto.IdCollege} introuvable."
                });
            }

            var lienExiste = await _context.CollegeDomaines
                .AnyAsync(cd =>
                    cd.IdDomaine == domaine.IdDomaine &&
                    cd.IdCollege == collegeDto.IdCollege);

            if (lienExiste)
            {
                continue;
            }

            _context.CollegeDomaines.Add(new CollegeDomaine
            {
                IdDomaine = domaine.IdDomaine,
                IdCollege = collegeDto.IdCollege,
                AccepteStagiaires = collegeDto.AccepteStagiaires,
                Actif = collegeDto.Actif
            });
        }

        await _context.SaveChangesAsync();

        var response = await GetDomaineCompletAsync(domaine.IdDomaine);
        return CreatedAtAction(nameof(GetDomaineEtudeById), new { id = domaine.IdDomaine }, response);
    }

    private async Task<DomaineEtudeResponseDto?> GetDomaineCompletAsync(int idDomaine)
    {
        var domaine = await _context.DomainesEtudes
            .AsNoTracking()
            .Include(d => d.CollegeDomaines)
                .ThenInclude(cd => cd.College)
            .FirstOrDefaultAsync(d => d.IdDomaine == idDomaine);

        return domaine == null ? null : MapDomaineResponse(domaine);
    }

    private static DomaineEtudeResponseDto MapDomaineResponse(DomaineEtude domaine)
    {
        var premierLienActif = domaine.CollegeDomaines
            .Where(cd => cd.College != null)
            .OrderBy(cd => cd.College!.Nom)
            .FirstOrDefault();

        return new DomaineEtudeResponseDto
        {
            IdDomaine = domaine.IdDomaine,
            Nom = domaine.Nom,
            Code = domaine.Code,
            Actif = domaine.Actif,

            Colleges = domaine.CollegeDomaines
                .Where(cd => cd.College != null)
                .OrderBy(cd => cd.College!.Nom)
                .Select(cd => new CollegeDomaineResponseDto
                {
                    IdCollege = cd.IdCollege,
                    NomCollege = cd.College!.Nom,
                    AccepteStagiaires = cd.AccepteStagiaires,
                    Actif = cd.Actif
                })
                .ToList(),

            // Champs de compatibilite temporaire pour les anciennes pages.
            IdCollege = premierLienActif?.IdCollege,
            NomCollege = premierLienActif?.College?.Nom,
            AccepteStagiaires = premierLienActif?.AccepteStagiaires
        };
    }
}