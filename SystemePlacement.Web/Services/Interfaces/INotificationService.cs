using SystemePlacement.Web.DTOs.Notifications;

namespace SystemePlacement.Web.Services.Interfaces;

public interface INotificationService
{
    Task NotifierUtilisateurAsync(int idUtilisateur, string message, string? lien = null);
    Task NotifierEmployeurAsync(int idEmployeur, string message, string? lien = null);
    Task NotifierEtudiantAsync(int idEtudiant, string message, string? lien = null);
    Task NotifierResponsablesCollegeAsync(int idCollege, string message, string? lien = null);
    Task<IReadOnlyList<NotificationResponse>> GetMesNotificationsAsync();
    Task<int> CompterMesNonLuesAsync();
    Task<bool> MarquerLueAsync(int idNotification);
}
