namespace SystemePlacement.Web.Services.Interfaces;

public interface ICurrentUserService
{
    int? IdUtilisateur { get; }
    string? Role { get; }
}