using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using SystemePlacement.Web.Controllers;

namespace SystemePlacement.Tests;

public class RecommandationsControllerAuthorizationTests
{
    [Fact]
    public void GetByEtudiant_DoesNotAuthorizeEmployeur()
    {
        var roles = GetAuthorizedRoles(
            nameof(RecommandationsController.GetByEtudiant));

        Assert.DoesNotContain("Employeur", roles);
        Assert.DoesNotContain("Etudiant", roles);
        Assert.Contains("ResponsableStage", roles);
        Assert.Contains("Administrateur", roles);
        Assert.Contains("SuperAdministrateur", roles);
    }

    [Fact]
    public void Creer_DoesNotAuthorizeEmployeur()
    {
        var roles = GetAuthorizedRoles(
            nameof(RecommandationsController.Creer));

        Assert.DoesNotContain("Employeur", roles);
        Assert.DoesNotContain("Etudiant", roles);
        Assert.Contains("ResponsableStage", roles);
        Assert.Contains("Administrateur", roles);
        Assert.Contains("SuperAdministrateur", roles);
    }

    [Fact]
    public void TelechargerLettre_AllowsIntendedManagementRoles()
    {
        var roles = GetAuthorizedRoles(
            nameof(RecommandationsController.TelechargerLettre));

        Assert.Contains("Employeur", roles);
        Assert.Contains("ResponsableStage", roles);
        Assert.Contains("Administrateur", roles);
        Assert.Contains("SuperAdministrateur", roles);
        Assert.DoesNotContain("Etudiant", roles);
    }

    [Fact]
    public void Supprimer_DoesNotAuthorizeEmployeur()
    {
        var roles = GetAuthorizedRoles(
            nameof(RecommandationsController.Supprimer));

        Assert.DoesNotContain("Employeur", roles);
        Assert.DoesNotContain("Etudiant", roles);
        Assert.Contains("ResponsableStage", roles);
        Assert.Contains("Administrateur", roles);
        Assert.Contains("SuperAdministrateur", roles);
    }

    private static string[] GetAuthorizedRoles(
        string methodName)
    {
        var method = typeof(RecommandationsController)
            .GetMethod(methodName);

        Assert.NotNull(method);

        var authorizeAttribute = method!
            .GetCustomAttributes<AuthorizeAttribute>()
            .Single();

        return (authorizeAttribute.Roles ?? string.Empty)
            .Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries);
    }
}