using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Colleges;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/colleges")]
public class CollegesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CollegesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CollegeResponseDto>>> GetColleges(bool includeInactive = false)
    {
        var query = _context.Colleges.AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(c => c.Actif);
        }

        var colleges = await query
            .OrderBy(c => c.Nom)
            .Select(c => MapCollegeResponse(c))
            .ToListAsync();

        return Ok(colleges);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CollegeResponseDto>> GetCollegeById(int id)
    {
        var college = await _context.Colleges
            .AsNoTracking()
            .Where(c => c.IdCollege == id)
            .Select(c => MapCollegeResponse(c))
            .FirstOrDefaultAsync();

        if (college == null)
        {
            return NotFound(new { message = $"College avec ID {id} non trouve." });
        }

        return Ok(college);
    }

    [HttpPost]
    public async Task<ActionResult<CollegeResponseDto>> CreateCollege([FromBody] CollegeCreateDto dto)
    {
        var nom = dto.Nom.Trim();
        var ville = dto.Ville.Trim();

        if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(ville))
        {
            return BadRequest(new { message = "Le nom et la ville sont obligatoires." });
        }

        var nomExiste = await _context.Colleges.AnyAsync(c => c.Nom == nom);

        if (nomExiste)
        {
            return BadRequest(new { message = $"Un college avec le nom '{nom}' existe deja." });
        }

        var college = new College
        {
            Nom = nom,
            Ville = ville,
            Actif = dto.Actif,

            // Theme visuel du college.
            CouleurPrimaire = dto.CouleurPrimaire,
            CouleurPrimaireFoncee = dto.CouleurPrimaireFoncee,
            CouleurSecondaire = dto.CouleurSecondaire,
            CouleurAccent = dto.CouleurAccent,
            CouleurFond = dto.CouleurFond,
            CouleurTexte = dto.CouleurTexte,
            LogoUrl = dto.LogoUrl
        };

        _context.Colleges.Add(college);
        await _context.SaveChangesAsync();

        var response = MapCollegeResponse(college);

        return CreatedAtAction(nameof(GetCollegeById), new { id = college.IdCollege }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCollege(int id, [FromBody] CollegeUpdateDto dto)
    {
        var nom = dto.Nom.Trim();
        var ville = dto.Ville.Trim();

        if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(ville))
        {
            return BadRequest(new { message = "Le nom et la ville sont obligatoires." });
        }

        var existingCollege = await _context.Colleges
            .FirstOrDefaultAsync(c => c.IdCollege == id);

        if (existingCollege == null)
        {
            return NotFound(new { message = $"College avec ID {id} non trouve." });
        }

        var nomExiste = await _context.Colleges
            .AnyAsync(c => c.Nom == nom && c.IdCollege != id);

        if (nomExiste)
        {
            return BadRequest(new { message = $"Un autre college avec le nom '{nom}' existe deja." });
        }

        existingCollege.Nom = nom;
        existingCollege.Ville = ville;
        existingCollege.Actif = dto.Actif;

        // Sauvegarde des couleurs choisies dans Gestion des colleges.
        existingCollege.CouleurPrimaire = dto.CouleurPrimaire;
        existingCollege.CouleurPrimaireFoncee = dto.CouleurPrimaireFoncee;
        existingCollege.CouleurSecondaire = dto.CouleurSecondaire;
        existingCollege.CouleurAccent = dto.CouleurAccent;
        existingCollege.CouleurFond = dto.CouleurFond;
        existingCollege.CouleurTexte = dto.CouleurTexte;
        existingCollege.LogoUrl = dto.LogoUrl;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCollege(int id)
    {
        var college = await _context.Colleges
            .FirstOrDefaultAsync(c => c.IdCollege == id);

        if (college == null)
        {
            return NotFound(new { message = $"College avec ID {id} non trouve." });
        }

        college.Actif = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static CollegeResponseDto MapCollegeResponse(College college)
    {
        return new CollegeResponseDto
        {
            IdCollege = college.IdCollege,
            Nom = college.Nom,
            Ville = college.Ville,
            Actif = college.Actif,
            CouleurPrimaire = college.CouleurPrimaire,
            CouleurPrimaireFoncee = college.CouleurPrimaireFoncee,
            CouleurSecondaire = college.CouleurSecondaire,
            CouleurAccent = college.CouleurAccent,
            CouleurFond = college.CouleurFond,
            CouleurTexte = college.CouleurTexte,
            LogoUrl = college.LogoUrl
        };
    }
}