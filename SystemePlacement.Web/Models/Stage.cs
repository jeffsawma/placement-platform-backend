using System.ComponentModel.DataAnnotations;

namespace SystemePlacement.Web.Models;

public class Stage
{
    [Key]
    public int IdStage { get; set; }

    // Etudiant place en stage.
    public int IdEtudiant { get; set; }

    public Etudiant? Etudiant { get; set; }

    // Offre de stage associee au placement.
    public int? IdOffre { get; set; }

    public Offre? Offre { get; set; }

    public DateTime? DateDebut { get; set; }

    public DateTime? DateFin { get; set; }

    public string? Lieu { get; set; }

    public string? Superviseur { get; set; }

    // EnAttente, Confirme, Refuse, Annule.
    public string Statut { get; set; } = "EnAttente";

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public DateTime? DateConfirmation { get; set; }

    public ICollection<ConfirmationStage> Confirmations { get; set; } = new List<ConfirmationStage>();
}