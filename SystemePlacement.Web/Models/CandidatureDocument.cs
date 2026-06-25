using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.Models;

public class CandidatureDocument
{
    public int IdDocument { get; set; }

    public int IdCandidature { get; set; }

    public TypeDocument TypeDocument { get; set; }

    public string NomFichier { get; set; } = string.Empty;

    public string CheminFichier { get; set; } = string.Empty;

    public string? ContentType { get; set; }

    public long TailleFichier { get; set; }

    public DateTime DateUpload { get; set; } = DateTime.UtcNow;

    public Candidature? Candidature { get; set; }
}
