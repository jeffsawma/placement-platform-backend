namespace SystemePlacement.Web.Models;

/// <summary>
/// Sous-type d'offre pour les postes d'emploi permanents/contractuels.
/// </summary>
public class OffreEmploi : Offre
{
    public string? TypeContrat { get; set; }       // ex: Temps plein, Temps partiel, Contractuel

    public decimal? SalaireMin { get; set; }

    public decimal? SalaireMax { get; set; }

    public string? TeleTravail { get; set; }       // Aucun, Partiel, Total
}
