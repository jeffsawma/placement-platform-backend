namespace SystemePlacement.Web.DTOs.DomainesEtudes;

public class CollegeDomaineUpdateDto
{
    // Indique si ce college accepte les stagiaires pour ce domaine.
    public bool AccepteStagiaires { get; set; } = true;

    // Active ou desactive ce domaine seulement pour ce college.
    public bool Actif { get; set; } = true;
}