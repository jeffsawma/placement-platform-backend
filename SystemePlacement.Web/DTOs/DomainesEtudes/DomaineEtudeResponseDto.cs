namespace SystemePlacement.Web.DTOs.DomainesEtudes
{
    public class DomaineEtudeResponseDto
    {
        public int IdDomaine { get; set; }
        public int IdCollege { get; set; }
        public string NomCollege { get; set; } = string.Empty; // Pour le frontend, on veut aussi le nom du collège associé
        public string Nom { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool AccepteStagiaires { get; set; }
        public bool Actif { get; set; }
    }
}

// Un example d'affichage pourrait être:
// {
//     "idDomaine": 1,
//     "idCollege": 2,
//     "nomCollege": "Cégep Gérald Godin",
//     "nom": "Programmation en technologies Web",
//     "code": "WEB101",
//     "accepteStagiaires": true,
//     "actif": true
// }
