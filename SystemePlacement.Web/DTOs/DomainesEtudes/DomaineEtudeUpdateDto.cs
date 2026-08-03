namespace SystemePlacement.Web.DTOs.DomainesEtudes;

public class DomaineEtudeUpdateDto
{
    // Nom global du domaine, exemple : Informatique.
    public string Nom { get; set; } = string.Empty;

    // Code global du domaine, exemple : INFO.
    public string Code { get; set; } = string.Empty;

    // Permet d'activer ou desactiver le domaine global.
    public bool Actif { get; set; } = true;
}