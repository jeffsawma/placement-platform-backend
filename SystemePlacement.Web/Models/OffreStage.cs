namespace SystemePlacement.Web.Models;

/// <summary>
/// Sous-type d'offre pour les stages (session, durée déterminée).
/// </summary>
public class OffreStage : Offre
{
    public DateTime? DateDebutStage { get; set; }

    public DateTime? DateFinStage { get; set; }

    public int? DureeHeuresParSemaine { get; set; }

    public decimal? Remuneration { get; set; }

    public string? Session { get; set; }           // ex: Automne 2025, Hiver 2026
}
