using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.Models;

public class Offre
{
    public int IdOffre { get; set; }

    public string Titre { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Ville { get; set; } = string.Empty;

    public string? Adresse { get; set; }

    public TypeOffre TypeOffre { get; set; }

    public StatutOffre Statut { get; set; } = StatutOffre.Active;

    public DateTime DatePublication { get; set; } = DateTime.UtcNow;

    public DateTime? DateExpiration { get; set; }

    public int IdEmployeur { get; set; }

    public Employeur? Employeur { get; set; }

    public ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();

    public ICollection<OffreDomaine> OffreDomaines { get; set; } = new List<OffreDomaine>();
}
