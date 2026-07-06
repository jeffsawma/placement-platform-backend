using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.Models;

public class OffreStageDirecte
{
    public int IdOffreDirecte { get; set; } // PK

    public int IdEtudiant { get; set; } // FK
    public int IdEmployeur { get; set; } // FK

    public int? IdOffreStage { get; set; } // FK
    public int? IdCandidature { get; set; } // FK
    public int? IdDemandeStage { get; set; } // FK

    public string Conditions { get; set; } = string.Empty;

    public DateTime? DateDebutProposee { get; set; }
    public DateTime? DateFinProposee { get; set; }
    public DateTime DateProposition { get; set; } = DateTime.UtcNow;

    public StatutOffreStageDirecte Statut { get; set; } = StatutOffreStageDirecte.Envoyee;

    public string? ReponseEtudiant { get; set; }
    public string? Commentaire { get; set; }

    // Relations
    public Etudiant? Etudiant { get; set; }
    public Employeur? Employeur { get; set; }

    public OffreStage? OffreStage { get; set; }
    public Candidature? Candidature { get; set; }
    public DemandeStage? DemandeStage { get; set; }
}
