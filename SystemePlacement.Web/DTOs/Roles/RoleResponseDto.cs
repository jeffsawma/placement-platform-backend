namespace SystemePlacement.Web.DTOs.Roles;

public class RoleResponseDto
{
    public int IdRole { get; set; }

    public string NomRole { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool Actif { get; set; }
}