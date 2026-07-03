using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Colleges;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/colleges")] // Route de base pour les collèges
public class CollegesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CollegesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/colleges
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CollegeResponseDto>>> GetColleges(bool includeInactive = false)
    {
        var query = _context.Colleges
            .AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(c => c.Actif);
        }

        var colleges = await query
            .OrderBy(c => c.Nom)
            .Select(c => new CollegeResponseDto
            {
                IdCollege = c.IdCollege,
                Nom = c.Nom,
                Ville = c.Ville,
                Actif = c.Actif
            })
            .ToListAsync();

        return Ok(colleges); // Retourne la liste de tous les collèges
    }

    // GET /api/colleges/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CollegeResponseDto>> GetCollegeById(int id)
    {
        var college = await _context.Colleges
            .AsNoTracking()
            .Where(c => c.IdCollege == id)
            .Select(c => new CollegeResponseDto
            {
                IdCollege = c.IdCollege,
                Nom = c.Nom,
                Ville = c.Ville,
                Actif = c.Actif
            })
            .FirstOrDefaultAsync();

        if (college == null)
        {
            return NotFound(new { message = $"Collège avec ID {id} non trouvé." });
        }

        return Ok(college);
    }

    // POST /api/colleges
    [HttpPost]
    public async Task<ActionResult<CollegeResponseDto>> CreateCollege([FromBody] CollegeCreateDto dto)
    {
        var nom = dto.Nom.Trim();
        var ville = dto.Ville.Trim();

        if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(ville))
        {
            return BadRequest(new { message = "Le nom et la ville sont obligatoires." });
        }

        // Vérifier si un collège avec le même nom existe déjà
        var nomExiste = await _context.Colleges
            .AnyAsync(c => c.Nom == nom);

        if (nomExiste)
        {
            return BadRequest(new { message = $"Un collège avec le nom '{nom}' existe déjà." });
        }

        var college = new College
        {
            Nom = nom, // Trim pour éviter les espaces inutiles
            Ville = ville,
            Actif = dto.Actif
        };


        _context.Colleges.Add(college);
        await _context.SaveChangesAsync();

        var response = new CollegeResponseDto
        {
            IdCollege = college.IdCollege,
            Nom = college.Nom,
            Ville = college.Ville,
            Actif = college.Actif
        };

        return CreatedAtAction(nameof(GetCollegeById), new { id = college.IdCollege }, response); // CreatedAtAction retourne un code 201 avec l'URL du nouvel élément créé et les données de ce dernier
    }

    // PUT /api/colleges/{id}
    [HttpPut("{id}")]
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
            return NotFound(new { message = $"Collège avec ID {id} non trouvé." });
        }

        // Vérifier si un autre collège avec le même nom existe déjà (en excluant le collège actuel)
        var nomExiste = await _context.Colleges
            .AnyAsync(c => c.Nom == nom && c.IdCollege != id);

        if (nomExiste)
        {
            return BadRequest(new { message = $"Un autre collège avec le nom '{nom}' existe déjà." });
        }


        // Mettre à jour les propriétés du collège existant avec les nouvelles valeurs
        existingCollege.Nom = nom;
        existingCollege.Ville = ville;
        existingCollege.Actif = dto.Actif;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /api/colleges/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCollege(int id)
    {
        var college = await _context.Colleges
            .FirstOrDefaultAsync(c => c.IdCollege == id);

        if (college == null)
        {
            return NotFound(new { message = $"Collège avec ID {id} non trouvé." });
        }

        college.Actif = false; // Marquer le collège comme inactif au lieu de le supprimer physiquement
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

// Ce controller gerera les routes liées aux colleges, comme :
// - GET /api/colleges : Récupérer la liste de tous les collèges
// - GET /api/colleges/{id} : Récupérer les détails d'un collège spécifique
// - POST /api/colleges : Créer un nouveau collège
// - PUT /api/colleges/{id} : Mettre à jour les informations d'un collège existant
// - DELETE /api/colleges/{id} : Supprimer un collège
