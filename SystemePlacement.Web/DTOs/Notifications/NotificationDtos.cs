namespace SystemePlacement.Web.DTOs.Notifications;

public class NotificationResponse
{
    public int IdNotification { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Lien { get; set; }
    public bool Lue { get; set; }
    public DateTime DateCreation { get; set; }
}
