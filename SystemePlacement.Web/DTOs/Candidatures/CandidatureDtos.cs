using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.DTOs.Candidatures;

public class PostulerRequest
{
    public int IdOffre { get; set; }
    public string? CvUrl { get; set; }
    public string? LettreUrl { get; set; }
    public string? LettreMotivation { get; set; }
}

public class ValidationCandidatureResponse
{
    public bool PeutPostuler { get; set; }
    public string? Message { get; set; }
}

public class ChangerStatutRequest
{
    public StatutCandidature Statut { get; set; }
}

public class MettreAJourCandidatureRequest
{
    public string? Message { get; set; }
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
    public string? MessageMotivation { get; set; }
    public string? MessageReponseEmployeur { get; set; }
    public DateTime? DateReponseEmployeur { get; set; }

    // Confirmation d'emploi : utilisé pour les offres d'emploi.
    public bool EmploiConfirme { get; set; }
    public string? MessageConfirmationEmploi { get; set; }
    public DateTime? DateConfirmationEmploi { get; set; }

    // Confirmation de stage : calculé à partir de la table stages + confirmations_stage.
    public bool StageConfirme { get; set; }
    public int NombreConfirmationsStage { get; set; }
    public string? StatutStage { get; set; }
    public DateTime? DateConfirmationStage { get; set; }
}

public class CandidatureResumeeResponse
{
    public int IdCandidature { get; set; }
    public int IdOffre { get; set; }
    public int IdEtudiant { get; set; }
    public string TitreOffre { get; set; } = string.Empty;
    public string NomEtudiant { get; set; } = string.Empty;
    public string PrenomEtudiant { get; set; } = string.Empty;
    public string? CourrielEtudiant { get; set; }
    public StatutCandidature Statut { get; set; }
    public DateTime DateCandidature { get; set; }
    public string? MessageMotivation { get; set; }
    public string? MessageReponseEmployeur { get; set; }
    public DateTime? DateReponseEmployeur { get; set; }
    public bool ACV { get; set; }
    public bool ALettreMotivation { get; set; }

    // Confirmation d'emploi : utilisé pour les offres d'emploi.
    public bool EmploiConfirme { get; set; }
    public string? MessageConfirmationEmploi { get; set; }
    public DateTime? DateConfirmationEmploi { get; set; }

    // Confirmation de stage : utilisé pour les offres de stage.
    public bool StageConfirme { get; set; }
    public int NombreConfirmationsStage { get; set; }
    public string? StatutStage { get; set; }
    public DateTime? DateConfirmationStage { get; set; }
}

public class CandidatureDetailResponse : CandidatureResumeeResponse
{
    public List<DocumentResponse> Documents { get; set; } = new();
}

public class DocumentResponse
{
    public int IdDocument { get; set; }
    public TypeDocument TypeDocument { get; set; }
    public string NomFichier { get; set; } = string.Empty;
    public long TailleFichier { get; set; }
    public DateTime DateUpload { get; set; }
}

public class ChangerStatutCandidatureRequest
{
    public StatutCandidature Statut { get; set; }
    public string? Message { get; set; }
}

public class ConfirmerEmploiRequest
{
    public string? Message { get; set; }
}