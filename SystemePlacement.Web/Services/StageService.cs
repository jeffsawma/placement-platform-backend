using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Stages;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class StageService : IStageService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public StageService(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<StageResponseDto> CreerStageAsync(StageCreateDto request)
    {
        var etudiantExiste = await _context.Etudiants
            .AnyAsync(e => e.IdEtudiant == request.IdEtudiant);

        if (!etudiantExiste)
        {
            throw new InvalidOperationException("Etudiant introuvable.");
        }

        if (request.IdOffre.HasValue)
        {
            var offreExiste = await _context.Offres
                .AnyAsync(o => o.IdOffre == request.IdOffre.Value);

            if (!offreExiste)
            {
                throw new InvalidOperationException("Offre introuvable.");
            }
        }

        var stage = new Stage
        {
            IdEtudiant = request.IdEtudiant,
            IdOffre = request.IdOffre,
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
            Lieu = request.Lieu,
            Superviseur = request.Superviseur,
            Statut = "EnAttente",
            DateCreation = DateTime.UtcNow
        };

        await _context.Stages.AddAsync(stage);
        await _context.SaveChangesAsync();

        return (await GetStageByIdAsync(stage.IdStage))!;
    }

    public async Task<StageResponseDto?> GetStageByIdAsync(int idStage)
    {
        return await _context.Stages
            .AsNoTracking()
            .Include(s => s.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Include(s => s.Offre)
            .Include(s => s.Confirmations)
                .ThenInclude(c => c.Utilisateur)
            .Where(s => s.IdStage == idStage)
            .Select(s => new StageResponseDto
            {
                IdStage = s.IdStage,
                IdEtudiant = s.IdEtudiant,
                NomEtudiant = s.Etudiant != null && s.Etudiant.Utilisateur != null
                    ? s.Etudiant.Utilisateur.Prenom + " " + s.Etudiant.Utilisateur.Nom
                    : string.Empty,
                IdOffre = s.IdOffre,
                TitreOffre = s.Offre != null ? s.Offre.Titre : null,
                DateDebut = s.DateDebut,
                DateFin = s.DateFin,
                Lieu = s.Lieu,
                Superviseur = s.Superviseur,
                Statut = s.Statut,
                DateCreation = s.DateCreation,
                DateConfirmation = s.DateConfirmation,
                Confirmations = s.Confirmations
                    .OrderByDescending(c => c.DateDecision)
                    .Select(c => new ConfirmationStageResponseDto
                    {
                        IdConfirmation = c.IdConfirmation,
                        TypeConfirmation = c.TypeConfirmation,
                        Decision = c.Decision,
                        Motif = c.Motif,
                        DateDecision = c.DateDecision,
                        IdUtilisateur = c.IdUtilisateur,
                        NomUtilisateur = c.Utilisateur != null
                            ? c.Utilisateur.NomUtilisateur
                            : string.Empty
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<StageResponseDto>> GetStagesAsync()
    {
        var stages = await _context.Stages
            .AsNoTracking()
            .Include(s => s.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Include(s => s.Offre)
            .Include(s => s.Confirmations)
                .ThenInclude(c => c.Utilisateur)
            .OrderByDescending(s => s.DateCreation)
            .ToListAsync();

        return stages.Select(s => new StageResponseDto
        {
            IdStage = s.IdStage,
            IdEtudiant = s.IdEtudiant,
            NomEtudiant = s.Etudiant?.Utilisateur != null
                ? s.Etudiant.Utilisateur.Prenom + " " + s.Etudiant.Utilisateur.Nom
                : string.Empty,
            IdOffre = s.IdOffre,
            TitreOffre = s.Offre?.Titre,
            DateDebut = s.DateDebut,
            DateFin = s.DateFin,
            Lieu = s.Lieu,
            Superviseur = s.Superviseur,
            Statut = s.Statut,
            DateCreation = s.DateCreation,
            DateConfirmation = s.DateConfirmation,
            Confirmations = s.Confirmations
                .OrderByDescending(c => c.DateDecision)
                .Select(c => new ConfirmationStageResponseDto
                {
                    IdConfirmation = c.IdConfirmation,
                    TypeConfirmation = c.TypeConfirmation,
                    Decision = c.Decision,
                    Motif = c.Motif,
                    DateDecision = c.DateDecision,
                    IdUtilisateur = c.IdUtilisateur,
                    NomUtilisateur = c.Utilisateur?.NomUtilisateur ?? string.Empty
                })
                .ToList()
        }).ToList();
    }

    public async Task<StageResponseDto?> ConfirmerStageAsync(
    int idStage,
    ConfirmationStageCreateDto request)
    {
        var stage = await _context.Stages
            .Include(s => s.Confirmations)
            .FirstOrDefaultAsync(s => s.IdStage == idStage);

        if (stage == null)
        {
            return null;
        }

        var typeConfirmation = GetTypeConfirmation();

        var confirmationExiste = stage.Confirmations
            .Any(c => c.TypeConfirmation == typeConfirmation);

        if (confirmationExiste)
        {
            throw new InvalidOperationException("Ce type de confirmation existe deja pour ce stage.");
        }

        var decision = request.Decision.Trim();

        if (decision != "Accepte" && decision != "Refuse")
        {
            throw new InvalidOperationException("La decision doit etre Accepte ou Refuse.");
        }

        // Valide que le token contient bien l'id de l'utilisateur connecte.
        if (!_currentUser.IdUtilisateur.HasValue)
        {
            throw new InvalidOperationException("Utilisateur connecte introuvable.");
        }

        var confirmation = new ConfirmationStage
        {
            IdStage = idStage,
            TypeConfirmation = typeConfirmation,
            Decision = decision,
            Motif = request.Motif,
            DateDecision = DateTime.UtcNow,
            IdUtilisateur = _currentUser.IdUtilisateur.Value
        };

        await _context.ConfirmationsStage.AddAsync(confirmation);
        await _context.SaveChangesAsync();

        await MettreAJourStatutStageAsync(stage.IdStage);

        return await GetStageByIdAsync(stage.IdStage);
    }

    private string GetTypeConfirmation()
    {
        if (_currentUser.Role == "Employeur")
        {
            return "Employeur";
        }

        if (_currentUser.Role == "ResponsableStage")
        {
            return "ResponsableStage";
        }

        throw new InvalidOperationException("Seul un employeur ou un responsable de stage peut confirmer un stage.");
    }

    private async Task MettreAJourStatutStageAsync(int idStage)
    {
        var stage = await _context.Stages
            .Include(s => s.Confirmations)
            .FirstAsync(s => s.IdStage == idStage);

        if (stage.Confirmations.Any(c => c.Decision == "Refuse"))
        {
            stage.Statut = "Refuse";
            stage.DateConfirmation = null;
        }
        else
        {
            var employeurConfirme = stage.Confirmations
                .Any(c => c.TypeConfirmation == "Employeur" && c.Decision == "Accepte");

            var responsableConfirme = stage.Confirmations
                .Any(c => c.TypeConfirmation == "ResponsableStage" && c.Decision == "Accepte");

            if (employeurConfirme && responsableConfirme)
            {
                stage.Statut = "Confirme";
                stage.DateConfirmation = DateTime.UtcNow;
            }
            else
            {
                stage.Statut = "EnAttente";
                stage.DateConfirmation = null;
            }
        }

        await _context.SaveChangesAsync();
    }
}