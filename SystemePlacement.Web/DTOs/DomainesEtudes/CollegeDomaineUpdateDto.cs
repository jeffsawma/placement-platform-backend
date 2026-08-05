namespace SystemePlacement.Web.DTOs.DomainesEtudes;

public class CollegeDomaineUpdateDto
{
    // Indique si ce collège accepte les stagiaires pour ce domaine.
    public bool AccepteStagiaires { get; set; } = true;

    // Active ou désactive ce domaine seulement pour ce collège.
    public bool Actif { get; set; } = true;
}