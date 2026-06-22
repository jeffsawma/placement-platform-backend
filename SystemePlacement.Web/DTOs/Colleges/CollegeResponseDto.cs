namespace SystemePlacement.Web.DTOs.Colleges
{
    public class CollegeResponseDto
    {
        public int IdCollege { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Ville { get; set; } = string.Empty;
        public bool Actif { get; set; }
    }
}

// Décrit ce que l'API renvoie au client lors d'une demande d'informations sur un établissement d'enseignement supérieur

