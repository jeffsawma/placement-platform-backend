using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.Models;

public class Candidature
{
    public int IdCandidature { get; set; }

    public int IdOffre { get; set; }

    public int IdEtudiant { get; set; }

    public StatutCandidature Statut { get; set; } = StatutCandidature.EnAttente;

    public DateTime DateCandidature { get; set; } = DateTime.UtcNow;

    public string? MessageMotivation { get; set; }

    public Offre? Offre { get; set; }

    public Etudiant? Etudiant { get; set; }

    public ICollection<CandidatureDocument> Documents { get; set; } = new List<CandidatureDocument>();
}
