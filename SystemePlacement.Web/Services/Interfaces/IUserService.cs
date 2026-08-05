using SystemePlacement.Web.DTOs.Users;

namespace SystemePlacement.Web.Services.Interfaces;

public interface IUserService
{
    // Retourne tous les utilisateurs pour l'administration.
    Task<IEnumerable<UtilisateurResponseDto>> GetAllAsync();

    // Retourne un utilisateur precis par son identifiant.
    Task<UtilisateurResponseDto?> GetByIdAsync(int idUtilisateur);

    // Crée un utilisateur et hache son mot de passe avant sauvegarde.
    Task<UtilisateurResponseDto> CreateAsync(UtilisateurCreateDto request);

    // Modifie les informations générales d'un utilisateur.
    Task<bool> UpdateAsync(int idUtilisateur, UtilisateurUpdateDto request);

    // Active ou désactive un compte utilisateur.
    Task<bool> SetActifAsync(int idUtilisateur, bool actif);
}