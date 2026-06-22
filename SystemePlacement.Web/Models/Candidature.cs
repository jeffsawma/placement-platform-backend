using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.Models;

public class Candidature
{
    public int IdCandidature { get; set; }

    public int IdOffre { get; set; }

    public int IdEtudiant { get; set; }

    public DateTime DateCandidature { get; set; } = DateTime.UtcNow;

    public StatutCandidature Statut { get; set; } = StatutCandidature.EnAttente;

    public string? CvUrl { get; set; }

    public string? LettreMotivation { get; set; }

    public Etudiant? Etudiant { get; set; }
}
