namespace SystemePlacement.Web.DTOs.Entreprises
{
    public class EntrepriseCreateDto
    {
        public string Nom { get; set; } = string.Empty;
        public string Secteur { get; set;} = string.Empty;
        public string Adresse { get; set;} = string.Empty;
        public string? SiteWeb { get; set;}
        public string? Description { get; set;}
        public string? LogoUrl { get; set;}
    }
}
