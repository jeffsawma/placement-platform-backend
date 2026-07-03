namespace SystemePlacement.Web.DTOs.Suivis;

public class EtudiantSuiviDetailResponseDto
{
    // Informations principales de l'etudiant suivi.
    public int IdEtudiant { get; set; }

    public int IdUtilisateur { get; set; }

    public string Prenom { get; set; } = string.Empty;

    public string Nom { get; set; } = string.Empty;

    public string Courriel { get; set; } = string.Empty;

    public string? Programme { get; set; }

    public string? Telephone { get; set; }

    public string? NomCollege { get; set; }

    // Resume des candidatures de l'etudiant.
    public List<CandidatureSuiviDto> Candidatures { get; set; } = new();

    // Notes et demarches de suivi ajoutees par les responsables.
    public List<DemarcheSuiviResponseDto> Demarches { get; set; } = new();
}

public class CandidatureSuiviDto
{
    public int IdCandidature { get; set; }

    public int IdOffre { get; set; }

    public string TitreOffre { get; set; } = string.Empty;

    public string TypeOffre { get; set; } = string.Empty;

    public string Statut { get; set; } = string.Empty;

    public DateTime DateCandidature { get; set; }
}