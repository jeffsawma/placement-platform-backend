using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.DTOs.Candidatures;

// ── Reponses ──────────────────────────────────────────────────────────────────

public class CandidatureResumeeResponse
{
    public int IdCandidature { get; set; }
    public int IdOffre { get; set; }
    public string TitreOffre { get; set; } = string.Empty;
    public string NomEtudiant { get; set; } = string.Empty;
    public string PrenomEtudiant { get; set; } = string.Empty;
    public string? EmailEtudiant { get; set; }
    public StatutCandidature Statut { get; set; }
    public DateTime DateCandidature { get; set; }
    public bool ACV { get; set; }
    public bool ALettreMotivation { get; set; }
}

public class CandidatureDetailResponse : CandidatureResumeeResponse
{
    public string? MessageMotivation { get; set; }
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

// ── Requetes ──────────────────────────────────────────────────────────────────

public class ChangerStatutCandidatureRequest
{
    public StatutCandidature Statut { get; set; }
}
