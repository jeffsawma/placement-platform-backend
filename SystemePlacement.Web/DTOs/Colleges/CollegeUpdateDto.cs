using System.Diagnostics.Contracts;

namespace SystemePlacement.Web.DTOs.Colleges
{
    public class CollegeUpdateDto
    {
        public string Nom { get; set; } = string.Empty;
        public string Ville { get; set; } = string.Empty;
        public bool Actif { get; set; } = true;
    }
}

// Décrit ce que l'API attend du client lors de la mise à jour d'un établissement d'enseignement supérieur existant\

