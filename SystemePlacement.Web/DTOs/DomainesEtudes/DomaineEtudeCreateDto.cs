namespace SystemePlacement.Web.DTOs.DomainesEtudes
{
    public class DomaineEtudeCreateDto
    {
        public int IdCollege { get; set; } // Clé étrangère vers College
        public string Nom { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool AccepteStagiaires { get; set; } = true;
        public bool Actif { get; set; } = true;
    }
}
