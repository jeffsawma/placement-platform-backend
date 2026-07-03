namespace SystemePlacement.Web.DTOs.Stages;

public class ConfirmationStageCreateDto
{
    // Accepte ou Refuse.
    public string Decision { get; set; } = string.Empty;

    public string? Motif { get; set; }
}