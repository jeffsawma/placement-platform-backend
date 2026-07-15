using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Repositories.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<List<Notification>> GetByUtilisateurAsync(int idUtilisateur);
    Task<Notification?> GetByIdAsync(int idNotification);
    Task<int?> GetIdUtilisateurByEmployeurAsync(int idEmployeur);
    Task<int> CompterNonLuesAsync(int idUtilisateur);
    Task SaveChangesAsync();
}
