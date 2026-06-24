using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.DTOs.Offres;

public class RechercheOffresQuery
{
    public TypeOffre? Type { get; set; }

    public int? IdDomaine { get; set; }

    public string? Lieu { get; set; }

    public string? MotsCles { get; set; }
}

public class OffreResponse
{
    public int IdOffre { get; set; }

    public string Titre { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Lieu { get; set; }

    public TypeOffre TypeOffre { get; set; }

    public StatutOffre Statut { get; set; }

    public int IdEntreprise { get; set; }

    public int? NombrePostes { get; set; }

    public bool? Remunere { get; set; }

    public DateTime DatePublication { get; set; }

    public DateTime? DateExpiration { get; set; }
}

public class OffreStatutResponse
{
    public int IdOffre { get; set; }

    public StatutOffre Statut { get; set; }
}
