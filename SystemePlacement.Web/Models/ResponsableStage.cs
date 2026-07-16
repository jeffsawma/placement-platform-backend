namespace SystemePlacement.Web.Models;

public class ResponsableStage
{
    public int IdResponsable { get; set; }

    public int IdUtilisateur { get; set; }

    public Utilisateur? Utilisateur { get; set; }
}