using SystemePlacement.Web.DTOs.Auth;

namespace SystemePlacement.Web.Services.Interfaces;

public interface IAuthService
{
    // Valide les identifiants et retourne un JWT si la connexion reussit.
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

    // Retourne les informations de l'utilisateur connecte.
    Task<UtilisateurConnecteDto?> GetUtilisateurConnecteAsync(int idUtilisateur);
}