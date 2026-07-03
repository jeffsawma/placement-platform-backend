using System.ComponentModel.DataAnnotations;

namespace SystemePlacement.Web.Models;

public class ConfirmationStage
{
    [Key]
    public int IdConfirmation { get; set; }

    public int IdStage { get; set; }

    public Stage? Stage { get; set; }

    // Employeur ou ResponsableStage.
    public string TypeConfirmation { get; set; } = string.Empty;

    // Accepte ou Refuse.
    public string Decision { get; set; } = string.Empty;

    public string? Motif { get; set; }

    public DateTime DateDecision { get; set; } = DateTime.UtcNow;

    // Compte utilisateur qui a fait la confirmation.
    public int IdUtilisateur { get; set; }

    public Utilisateur? Utilisateur { get; set; }
}