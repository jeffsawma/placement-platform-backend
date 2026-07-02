using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.Models;

public class DemandeStage
{
    public int IdDemandeStage { get; set; }
    public int IdEtudiant { get; set; }
    public int IdDomaine { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? PeriodeSouhaitee { get; set; }
    public string? Competences { get; set; }
    public StatutDemandeStage Statut { get; set; } = StatutDemandeStage.Ouverte;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public Etudiant? Etudiant { get; set; }
    public DomaineEtude? DomaineEtude { get; set; }
}
