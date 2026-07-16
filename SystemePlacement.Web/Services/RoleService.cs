using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Roles;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;

    public RoleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoleResponseDto>> GetAllAsync()
    {
        // Retourne seulement les roles actifs, tries par nom.
        return await _context.Roles
            .Where(r => r.Actif)
            .OrderBy(r => r.NomRole)
            .Select(r => new RoleResponseDto
            {
                IdRole = r.IdRole,
                NomRole = r.NomRole,
                Description = r.Description,
                Actif = r.Actif
            })
            .ToListAsync();
    }
}