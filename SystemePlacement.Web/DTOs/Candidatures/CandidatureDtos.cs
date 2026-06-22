using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.DTOs.Candidatures;

public class PostulerRequest
{
    public int IdOffre { get; set; }

    public string? CvUrl { get; set; }

    public string? LettreMotivation { get; set; }
}

public class ChangerStatutRequest
{
    public StatutCandidature Statut { get; set; }
}

public class CandidatureResponse
{
    public int IdCandidature { get; set; }

    public int IdOffre { get; set; }

    public int IdEtudiant { get; set; }

    public DateTime DateCandidature { get; set; }

    public StatutCandidature Statut { get; set; }

    public string? CvUrl { get; set; }

    public string? LettreMotivation { get; set; }
}
