using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Auth;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Recherche l'utilisateur avec son role.
        var utilisateur = await _context.Utilisateurs
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.NomUtilisateur == request.NomUtilisateur &&
                u.Actif);

        if (utilisateur == null)
        {
            throw new UnauthorizedAccessException("Nom d'utilisateur ou mot de passe invalide.");
        }

        // Verifie le mot de passe avec BCrypt.
        var motDePasseValide = BCrypt.Net.BCrypt.Verify(
            request.MotDePasse,
            utilisateur.MotDePasseHash);

        if (!motDePasseValide)
        {
            throw new UnauthorizedAccessException("Nom d'utilisateur ou mot de passe invalide.");
        }

        utilisateur.DerniereConnexion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var expiration = DateTime.UtcNow.AddHours(8);
        var token = GenerateJwtToken(utilisateur.IdUtilisateur, utilisateur.NomUtilisateur, utilisateur.Role?.NomRole ?? string.Empty, expiration);

        return new LoginResponseDto
        {
            Token = token,
            Expiration = expiration,
            Utilisateur = new UtilisateurConnecteDto
            {
                IdUtilisateur = utilisateur.IdUtilisateur,
                Prenom = utilisateur.Prenom,
                Nom = utilisateur.Nom,
                Courriel = utilisateur.Courriel,
                NomUtilisateur = utilisateur.NomUtilisateur,
                Role = utilisateur.Role?.NomRole ?? string.Empty,
                IdCollege = utilisateur.IdCollege
            }
        };
    }

    public async Task<UtilisateurConnecteDto?> GetUtilisateurConnecteAsync(int idUtilisateur)
    {
        // Retourne les infos utiles pour /api/auth/me.
        return await _context.Utilisateurs
            .Include(u => u.Role)
            .Where(u => u.IdUtilisateur == idUtilisateur && u.Actif)
            .Select(u => new UtilisateurConnecteDto
            {
                IdUtilisateur = u.IdUtilisateur,
                Prenom = u.Prenom,
                Nom = u.Nom,
                Courriel = u.Courriel,
                NomUtilisateur = u.NomUtilisateur,
                Role = u.Role != null ? u.Role.NomRole : string.Empty,
                IdCollege = u.IdCollege
            })
            .FirstOrDefaultAsync();
    }

    private string GenerateJwtToken(int idUtilisateur, string nomUtilisateur, string role, DateTime expiration)
    {
        var secretKey = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("La cle JWT est manquante dans appsettings.json.");
        }

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var claims = new List<Claim>
        {
            // Identifiant de l'utilisateur connecte
            new Claim(ClaimTypes.NameIdentifier, idUtilisateur.ToString()),

            // Nom d'utilisateur
            new Claim(ClaimTypes.Name, nomUtilisateur),

            // Role utilise par [Authorize(Roles = "...")]
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}