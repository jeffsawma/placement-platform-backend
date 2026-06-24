namespace SystemePlacement.Web.DTOs.Colleges
{
    public class CollegeCreateDto
    {
        public string Nom { get; set; } = string.Empty;
        public string Ville { get; set; } = string.Empty;
        public bool Actif { get; set; } = true;
    }
}

// Décrit ce que l'API attend du client lors de la création d'un nouveau collège

