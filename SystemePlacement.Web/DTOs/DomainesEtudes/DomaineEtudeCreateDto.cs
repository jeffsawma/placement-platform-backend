namespace SystemePlacement.Web.DTOs.DomainesEtudes;

public class DomaineEtudeCreateDto
{
    // Nom global du domaine, exemple : Informatique.
    public string Nom { get; set; } = string.Empty;

    // Code global du domaine, exemple : INFO.
    public string Code { get; set; } = string.Empty;

    // Permet de desactiver le domaine global sans le supprimer.
    public bool Actif { get; set; } = true;

    // SuperAdmin : peut choisir plusieurs cegeps.
    // Admin : le backend va ignorer cette liste et utiliser automatiquement son college.
    public List<CollegeDomaineCreateDto> Colleges { get; set; } = new();
}