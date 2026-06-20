using SystemePlacement.Web.DTOs.Roles;

namespace SystemePlacement.Web.Services.Interfaces;

public interface IRoleService
{
    // Retourne les roles actifs disponibles dans la plateforme.
    Task<IEnumerable<RoleResponseDto>> GetAllAsync();
}