namespace SystemePlacement.Web.DTOs.Stages;

public class StageResponseDto
{
    public int IdStage { get; set; }

    public int IdEtudiant { get; set; }

    public string NomEtudiant { get; set; } = string.Empty;

    public int? IdOffre { get; set; }

    public string? TitreOffre { get; set; }

    public DateTime? DateDebut { get; set; }

    public DateTime? DateFin { get; set; }

    public string? Lieu { get; set; }

    public string? Superviseur { get; set; }

    public string Statut { get; set; } = string.Empty;

    public DateTime DateCreation { get; set; }

    public DateTime? DateConfirmation { get; set; }

    public List<ConfirmationStageResponseDto> Confirmations { get; set; } = new();
}

public class ConfirmationStageResponseDto
{
    public int IdConfirmation { get; set; }

    public string TypeConfirmation { get; set; } = string.Empty;

    public string Decision { get; set; } = string.Empty;

    public string? Motif { get; set; }

    public DateTime DateDecision { get; set; }

    public int IdUtilisateur { get; set; }

    public string NomUtilisateur { get; set; } = string.Empty;
}