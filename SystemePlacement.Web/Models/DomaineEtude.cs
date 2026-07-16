namespace SystemePlacement.Web.Models;

public class DomaineEtude
{
    public int IdDomaine { get; set; }
    public int IdCollege { get; set; } // Clé étrangère vers College
    public string Nom { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool AccepteStagiaires { get; set; } = true; // Par défaut, un domaine d'étude accepte les stagiaires
    public bool Actif { get; set; } = true; // Par défaut, un domaine d'étude est actif

    // Relations
    public College? College { get; set; } // Navigation vers College
}
