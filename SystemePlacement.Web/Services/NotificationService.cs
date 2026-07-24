using SystemePlacement.Web.DTOs.Notifications;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public NotificationService(
        INotificationRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task NotifierUtilisateurAsync(
        int idUtilisateur,
        string message)
    {
        if (idUtilisateur <= 0 || string.IsNullOrWhiteSpace(message))
            return;

        await _repository.AddAsync(new Notification
        {
            IdUtilisateur = idUtilisateur,
            Message = message.Trim(),
            Lue = false,
            DateCreation = DateTime.UtcNow
        });

        await _repository.SaveChangesAsync();
    }

    public async Task NotifierEmployeurAsync(
        int idEmployeur,
        string message)
    {
        var idUtilisateur =
            await _repository.GetIdUtilisateurByEmployeurAsync(idEmployeur);

        if (idUtilisateur is null)
            return;

        await NotifierUtilisateurAsync(idUtilisateur.Value, message);
    }

    public async Task<IReadOnlyList<NotificationResponse>>
        GetMesNotificationsAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue)
            return Array.Empty<NotificationResponse>();

        var notifications =
            await _repository.GetByUtilisateurAsync(
                _currentUser.IdUtilisateur.Value);

        return notifications.Select(Map).ToList();
    }

    public async Task<int> CompterMesNonLuesAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue)
            return 0;

        return await _repository.CompterNonLuesAsync(
            _currentUser.IdUtilisateur.Value);
    }

    public async Task<bool> MarquerLueAsync(int idNotification)
    {
        if (!_currentUser.IdUtilisateur.HasValue)
            return false;

        var notification =
            await _repository.GetByIdAsync(idNotification);

        if (notification is null ||
            notification.IdUtilisateur !=
            _currentUser.IdUtilisateur.Value)
        {
            return false;
        }

        notification.Lue = true;

        await _repository.SaveChangesAsync();

        return true;
    }

    private static NotificationResponse Map(Notification notification) => new()
    {
        IdNotification = notification.IdNotification,
        Message = notification.Message,
        Lue = notification.Lue,
        DateCreation = notification.DateCreation
    };
}
