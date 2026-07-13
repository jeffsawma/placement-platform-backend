namespace SystemePlacement.Web.DTOs.Notifications;

public class NotificationResponse
{
    public int IdNotification { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Lue { get; set; }
    public DateTime DateCreation { get; set; }
}
