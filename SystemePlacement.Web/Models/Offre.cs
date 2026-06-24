using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.Models;

public class Offre
{
    public int IdOffre { get; set; }

    public string Titre { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Lieu { get; set; }

    public DateTime DatePublication { get; set; } = DateTime.UtcNow;

    public DateTime? DateExpiration { get; set; }

    public StatutOffre Statut { get; set; } = StatutOffre.Brouillon;

    public TypeOffre TypeOffre { get; set; }

    public int IdEntreprise { get; set; }

    public int? NombrePostes { get; set; }

    public bool? Remunere { get; set; }

    public ICollection<OffreDomaine> OffreDomaines { get; set; } = new List<OffreDomaine>();
}
