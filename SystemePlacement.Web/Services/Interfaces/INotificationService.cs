using SystemePlacement.Web.DTOs.Notifications;

namespace SystemePlacement.Web.Services.Interfaces;

public interface INotificationService
{
    Task NotifierUtilisateurAsync(int idUtilisateur, string message);
    Task NotifierEmployeurAsync(int idEmployeur, string message);
    Task NotifierEtudiantAsync(int idEtudiant, string message);
    Task NotifierResponsablesCollegeAsync(int idCollege, string message);
    Task<IReadOnlyList<NotificationResponse>> GetMesNotificationsAsync();
    Task<int> CompterMesNonLuesAsync();
    Task<bool> MarquerLueAsync(int idNotification);
}
