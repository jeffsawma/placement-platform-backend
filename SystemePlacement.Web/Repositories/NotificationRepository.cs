using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;

namespace SystemePlacement.Web.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Notification notification) =>
        await _context.Notifications.AddAsync(notification);

    public Task<List<Notification>> GetByUtilisateurAsync(int idUtilisateur) =>
        _context.Notifications
            .AsNoTracking()
            .Where(n => n.IdUtilisateur == idUtilisateur)
            .OrderByDescending(n => n.DateCreation)
            .ToListAsync();

    public Task<Notification?> GetByIdAsync(int idNotification) =>
        _context.Notifications.FirstOrDefaultAsync(n => n.IdNotification == idNotification);

    public Task<int?> GetIdUtilisateurByEmployeurAsync(int idEmployeur) =>
        _context.Employeurs
            .Where(e => e.IdEmployeur == idEmployeur)
            .Select(e => (int?)e.IdUtilisateur)
            .FirstOrDefaultAsync();

    public Task<int> CompterNonLuesAsync(int idUtilisateur) =>
        _context.Notifications.CountAsync(n => n.IdUtilisateur == idUtilisateur && !n.Lue);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
