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
    private readonly INotificationService _notification;

    public StageService(
        ApplicationDbContext context,
        ICurrentUserService currentUser,
        INotificationService notification)
    {
        _context = context;
        _currentUser = currentUser;
        _notification = notification;
    }

    public async Task<StageResponseDto> CreerStageAsync(StageCreateDto request)
    {
        var etudiantExiste = await _context.Etudiants
            .AnyAsync(e => e.IdEtudiant == request.IdEtudiant);

        if (!etudiantExiste)
        {
            throw new InvalidOperationException("Étudiant introuvable.");
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
        var query = _context.Stages
            .AsNoTracking()
            .Include(s => s.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Include(s => s.Offre)
            .Include(s => s.Confirmations)
                .ThenInclude(c => c.Utilisateur)
            .Where(s => s.IdStage == idStage);

        query = await AppliquerPorteeUtilisateurAsync(query);

        var stage = await query.FirstOrDefaultAsync();

        return stage is null ? null : MapStageResponse(stage);
    }

    public async Task<IReadOnlyList<StageResponseDto>> GetStagesAsync()
    {
        var query = _context.Stages
            .AsNoTracking()
            .Include(s => s.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Include(s => s.Offre)
            .Include(s => s.Confirmations)
                .ThenInclude(c => c.Utilisateur)
            .AsQueryable();

        query = await AppliquerPorteeUtilisateurAsync(query);

        var stages = await query
            .OrderByDescending(s => s.DateCreation)
            .ToListAsync();

        return stages.Select(MapStageResponse).ToList();
    }

    public async Task<StageResponseDto?> ConfirmerStageAsync(
        int idStage,
        ConfirmationStageCreateDto request)
    {
        var stage = await _context.Stages
            .Include(s => s.Offre)
            .Include(s => s.Confirmations)
            .FirstOrDefaultAsync(s => s.IdStage == idStage);

        if (stage == null)
        {
            return null;
        }

        await ValiderDroitConfirmationAsync(stage);

        var typeConfirmation = GetTypeConfirmation();

        var confirmationExiste = stage.Confirmations
            .Any(c => c.TypeConfirmation == typeConfirmation);

        if (confirmationExiste)
        {
            throw new InvalidOperationException("Ce type de confirmation existe déjà pour ce stage.");
        }

        var decision = request.Decision.Trim();

        if (decision != "Accepte" && decision != "Refuse")
        {
            throw new InvalidOperationException("La décision doit être Accepte ou Refuse.");
        }

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

    private async Task<IQueryable<Stage>> AppliquerPorteeUtilisateurAsync(IQueryable<Stage> query)
    {
        // Le SuperAdmin et l'Admin peuvent voir tous les stages pour le moment.
        if (_currentUser.Role == "SuperAdministrateur" ||
            _currentUser.Role == "Administrateur")
        {
            return query;
        }

        // Un employeur voit seulement les stages lies a ses offres.
        if (_currentUser.Role == "Employeur")
        {
            if (!_currentUser.IdUtilisateur.HasValue)
            {
                return query.Where(s => false);
            }

            var idEmployeur = await _context.Employeurs
                .Where(e => e.IdUtilisateur == _currentUser.IdUtilisateur.Value)
                .Select(e => (int?)e.IdEmployeur)
                .FirstOrDefaultAsync();

            if (idEmployeur is null)
            {
                return query.Where(s => false);
            }

            return query.Where(s =>
                s.Offre != null &&
                s.Offre.IdEmployeur == idEmployeur.Value);
        }

        // Un responsable voit seulement les stages des étudiants de son collège.
        if (_currentUser.Role == "ResponsableStage")
        {
            if (!_currentUser.IdCollege.HasValue)
            {
                return query.Where(s => false);
            }

            return query.Where(s =>
                s.Etudiant != null &&
                s.Etudiant.Utilisateur != null &&
                s.Etudiant.Utilisateur.IdCollege == _currentUser.IdCollege.Value);
        }

        return query.Where(s => false);
    }

    private async Task ValiderDroitConfirmationAsync(Stage stage)
    {
        if (_currentUser.Role == "Employeur")
        {
            if (!_currentUser.IdUtilisateur.HasValue)
            {
                throw new InvalidOperationException("Utilisateur connecte introuvable.");
            }

            var idEmployeur = await _context.Employeurs
                .Where(e => e.IdUtilisateur == _currentUser.IdUtilisateur.Value)
                .Select(e => (int?)e.IdEmployeur)
                .FirstOrDefaultAsync();

            if (idEmployeur is null ||
                stage.Offre == null ||
                stage.Offre.IdEmployeur != idEmployeur.Value)
            {
                throw new InvalidOperationException("Vous ne pouvez pas confirmer un stage qui ne correspond pas a votre offre.");
            }
        }

        if (_currentUser.Role == "ResponsableStage")
        {
            if (!_currentUser.IdCollege.HasValue)
            {
                throw new InvalidOperationException("Votre compte responsable n'est rattache a aucun college.");
            }

            var stageDuCollege = await _context.Stages
                .Include(s => s.Etudiant)
                    .ThenInclude(e => e!.Utilisateur)
                .AnyAsync(s =>
                    s.IdStage == stage.IdStage &&
                    s.Etudiant != null &&
                    s.Etudiant.Utilisateur != null &&
                    s.Etudiant.Utilisateur.IdCollege == _currentUser.IdCollege.Value);

            if (!stageDuCollege)
            {
                throw new InvalidOperationException("Vous ne pouvez pas confirmer un stage d'un autre college.");
            }
        }
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
            .Include(s => s.Offre)
            .Include(s => s.Confirmations)
            .FirstAsync(s => s.IdStage == idStage);

        var ancienStatut = stage.Statut;

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

        if (stage.Statut == ancienStatut)
            return;

        var libelleStage = string.IsNullOrWhiteSpace(stage.Offre?.Titre)
            ? "Votre stage"
            : $"Votre stage pour « {stage.Offre.Titre} »";

        if (stage.Statut == "Confirme")
        {
            await _notification.NotifierEtudiantAsync(
                stage.IdEtudiant,
                $"{libelleStage} a été officiellement confirmé par l'employeur et le responsable de stage.");
        }
        else if (stage.Statut == "Refuse")
        {
            await _notification.NotifierEtudiantAsync(
                stage.IdEtudiant,
                $"La confirmation de {libelleStage.ToLowerInvariant()} a été refusée.");
        }
    }

    private static StageResponseDto MapStageResponse(Stage stage)
    {
        return new StageResponseDto
        {
            IdStage = stage.IdStage,
            IdEtudiant = stage.IdEtudiant,
            NomEtudiant = stage.Etudiant?.Utilisateur != null
                ? stage.Etudiant.Utilisateur.Prenom + " " + stage.Etudiant.Utilisateur.Nom
                : string.Empty,
            IdOffre = stage.IdOffre,
            TitreOffre = stage.Offre?.Titre,
            DateDebut = stage.DateDebut,
            DateFin = stage.DateFin,
            Lieu = stage.Lieu,
            Superviseur = stage.Superviseur,
            Statut = stage.Statut,
            DateCreation = stage.DateCreation,
            DateConfirmation = stage.DateConfirmation,
            Confirmations = stage.Confirmations
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
        };
    }
}