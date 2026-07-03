namespace SystemePlacement.Web.DTOs.Stages;

public class StageCreateDto
{
    public int IdEtudiant { get; set; }

    public int? IdOffre { get; set; }

    public DateTime? DateDebut { get; set; }

    public DateTime? DateFin { get; set; }

    public string? Lieu { get; set; }

    public string? Superviseur { get; set; }
}