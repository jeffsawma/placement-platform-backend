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
    public string? MessageMotivation { get; set; }

    // Added by Dev 2
    // 'MessageReponseEmployeur' est une propriété qui peut contenir un message de réponse de l'employeur à la candidature de l'étudiant
    // 'DateReponseEmployeur' est une propriété qui peut contenir la date à laquelle l'employeur a répondu à la candidature de l'étudiant
    public string? MessageReponseEmployeur { get; set; }
    public DateTime? DateReponseEmployeur { get; set; }

    public Offre? Offre { get; set; }
    public Etudiant? Etudiant { get; set; }

    public ICollection<CandidatureDocument> Documents { get; set; } = new List<CandidatureDocument>();
}
