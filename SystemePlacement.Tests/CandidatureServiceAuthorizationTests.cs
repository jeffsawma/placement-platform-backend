using SystemePlacement.Web.DTOs.Candidatures;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;
using SystemePlacement.Web.Services;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Tests;

public class CandidatureServiceAuthorizationTests
{
    [Fact]
    public async Task ChangerStatutAsync_DoesNotModifyCandidature_WhenEmployerDoesNotOwnOffer()
    {
        var offre = new OffreEmploi
        {
            IdOffre = 6,
            IdEmployeur = 10,
            Titre = "Offre appartenant à un autre employeur"
        };

        var candidature = new Candidature
        {
            IdCandidature = 9,
            IdOffre = offre.IdOffre,
            IdEtudiant = 100,
            Statut = StatutCandidature.EnAttente,
            Offre = offre
        };

        var candidatureRepository =
            new FakeCandidatureRepository(candidature);

        var offreRepository =
            new FakeOffreRepository(
                offre,
                connectedEmployerId: 20);

        var currentUser = new FakeCurrentUserService
        {
            IdUtilisateur = 500,
            Role = "Employeur"
        };

        var service = new CandidatureService(
            context: null!,
            repository: candidatureRepository,
            offreRepository: offreRepository,
            currentUser: currentUser,
            env: null!,
            notification: null!);

        var result = await service.ChangerStatutAsync(
            candidature.IdCandidature,
            StatutCandidature.Acceptee,
            "Tentative interdite");

        Assert.False(result);

        Assert.Equal(
            StatutCandidature.EnAttente,
            candidature.Statut);

        Assert.Null(candidature.MessageReponseEmployeur);
        Assert.Null(candidature.DateReponseEmployeur);

        Assert.False(candidatureRepository.UpdateCalled);
        Assert.False(candidatureRepository.SaveChangesCalled);
    }

    [Fact]
    public async Task MettreAJourAsync_DoesNotModifyCandidature_WhenStudentDoesNotOwnIt()
    {
        var candidature = new Candidature
        {
            IdCandidature = 9,
            IdOffre = 6,
            IdEtudiant = 200,
            Statut = StatutCandidature.EnAttente,
            MessageMotivation = "Message initial",
            LettreMotivation = "Message initial"
        };

        var candidatureRepository =
            new FakeCandidatureRepository(
                candidature,
                connectedStudentId: 100);

        var currentUser = new FakeCurrentUserService
        {
            IdUtilisateur = 500,
            Role = "Etudiant"
        };

        var service = new CandidatureService(
            context: null!,
            repository: candidatureRepository,
            offreRepository: null!,
            currentUser: currentUser,
            env: null!,
            notification: null!);

        var request = new MettreAJourCandidatureRequest
        {
            Message = "Tentative de modification interdite"
        };

        var result = await service.MettreAJourAsync(
            candidature.IdCandidature,
            request);

        Assert.False(result);
        Assert.Equal(
            "Message initial",
            candidature.MessageMotivation);

        Assert.Equal(
            "Message initial",
            candidature.LettreMotivation);

        Assert.False(candidatureRepository.UpdateCalled);
        Assert.False(candidatureRepository.SaveChangesCalled);
    }

    [Fact]
    public async Task RetirerAsync_DoesNotWithdrawCandidature_WhenStudentDoesNotOwnIt()
    {
        var candidature = new Candidature
        {
            IdCandidature = 9,
            IdOffre = 6,
            IdEtudiant = 200,
            Statut = StatutCandidature.EnAttente
        };

        var candidatureRepository =
            new FakeCandidatureRepository(
                candidature,
                connectedStudentId: 100);

        var currentUser = new FakeCurrentUserService
        {
            IdUtilisateur = 500,
            Role = "Etudiant"
        };

        var service = new CandidatureService(
            context: null!,
            repository: candidatureRepository,
            offreRepository: null!,
            currentUser: currentUser,
            env: null!,
            notification: null!);

        var result = await service.RetirerAsync(
            candidature.IdCandidature);

        Assert.False(result);

        Assert.Equal(
            StatutCandidature.EnAttente,
            candidature.Statut);

        Assert.False(candidatureRepository.UpdateCalled);
        Assert.False(candidatureRepository.SaveChangesCalled);
    }

    private sealed class FakeCurrentUserService
        : ICurrentUserService
    {
        public int? IdUtilisateur { get; init; }

        public int? IdCollege => null;

        public string? Role { get; init; }

        public bool IsAuthenticated => true;
    }

    private sealed class FakeCandidatureRepository
        : ICandidatureRepository
    {
        private readonly Candidature _candidature;
        private readonly int? _connectedStudentId;

        public FakeCandidatureRepository(
            Candidature candidature,
            int? connectedStudentId = null)
        {
            _candidature = candidature;
            _connectedStudentId = connectedStudentId;
        }

        public bool UpdateCalled { get; private set; }

        public bool SaveChangesCalled { get; private set; }

        public Task<List<Candidature>> GetByOffreAsync(
            int idOffre)
        {
            return Task.FromResult(
                new List<Candidature>());
        }

        public Task<List<Candidature>> GetByEtudiantAsync(
            int idEtudiant)
        {
            return Task.FromResult(
                new List<Candidature>());
        }

        public Task<List<Candidature>> GetByDomaineAsync(
            int idDomaine)
        {
            return Task.FromResult(
                new List<Candidature>());
        }

        public Task<Candidature?> GetByIdAsync(
            int idCandidature)
        {
            Candidature? result =
                idCandidature == _candidature.IdCandidature
                    ? _candidature
                    : null;

            return Task.FromResult(result);
        }

        public Task<bool> ExistsAsync(
            int idOffre,
            int idEtudiant)
        {
            return Task.FromResult(false);
        }

        public Task<int?> GetIdEtudiantByUtilisateurAsync(
            int idUtilisateur)
        {
            return Task.FromResult(
                _connectedStudentId);
        }

        public Task<string?> GetNomEmployeurAsync(
            int idEmployeur)
        {
            return Task.FromResult<string?>(null);
        }

        public Task AddAsync(
            Candidature candidature)
        {
            return Task.CompletedTask;
        }

        public Task<CandidatureDocument?> GetDocumentAsync(
            int idDocument)
        {
            return Task.FromResult<CandidatureDocument?>(null);
        }

        public void Update(
            Candidature candidature)
        {
            UpdateCalled = true;
        }

        public Task SaveChangesAsync()
        {
            SaveChangesCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeOffreRepository
        : IOffreRepository
    {
        private readonly Offre _offre;
        private readonly int _connectedEmployerId;

        public FakeOffreRepository(
            Offre offre,
            int connectedEmployerId)
        {
            _offre = offre;
            _connectedEmployerId = connectedEmployerId;
        }

        public Task<List<Offre>> GetAllAsync(
            TypeOffre? type = null,
            StatutOffre? statut = null,
            int? idDomaine = null,
            string? lieu = null,
            string? motsCles = null)
        {
            return Task.FromResult(
                new List<Offre>());
        }

        public Task<List<Offre>> GetByEmployeurAsync(
            int idEmployeur)
        {
            return Task.FromResult(
                new List<Offre>());
        }

        public Task<Offre?> GetByIdAsync(
            int idOffre)
        {
            Offre? result =
                idOffre == _offre.IdOffre
                    ? _offre
                    : null;

            return Task.FromResult(result);
        }

        public Task<int?> GetIdEmployeurByUtilisateurAsync(
            int idUtilisateur)
        {
            return Task.FromResult<int?>(
                _connectedEmployerId);
        }

        public Task AddAsync(
            Offre offre)
        {
            return Task.CompletedTask;
        }

        public void Update(
            Offre offre)
        {
        }

        public void Delete(
            Offre offre)
        {
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }

        public Task<List<OffreDomaine>> GetDomainesOffreAsync(
            int idOffre)
        {
            return Task.FromResult(
                new List<OffreDomaine>());
        }

        public void RemoveDomaines(
            IEnumerable<OffreDomaine> domaines)
        {
        }

        public Task AddDomainesAsync(
            IEnumerable<OffreDomaine> domaines)
        {
            return Task.CompletedTask;
        }
    }
}