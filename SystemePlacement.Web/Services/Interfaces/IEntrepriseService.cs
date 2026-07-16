using SystemePlacement.Web.DTOs.Entreprises;

namespace SystemePlacement.Web.Services.Interfaces
{
    public interface IEntrepriseService
    {
        Task<EntrepriseResponseDto?> GetMonProfilAsync();
        Task<EntrepriseResponseDto> CreateMonProfilAsync(EntrepriseCreateDto dto);
        Task<bool> UpdateMonProfilAsync(EntrepriseUpdateDto dto); // Retourne False si le profil de l'entreprise n'existe pas
    }
}
