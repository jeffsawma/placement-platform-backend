# Système de placement — Backend

API REST ASP.NET Core du système de placement en ligne du Cégep.

Le backend centralise l’authentification et les principales fonctionnalités de la plateforme : gestion des utilisateurs, des cégeps et des domaines d’études, profils d’entreprise, offres d’emploi et de stage, candidatures, demandes de stage, offres directes, confirmations et notifications.

## Technologies

- .NET 8 et ASP.NET Core Web API
- Entity Framework Core
- MySQL
- Authentification JWT

## Démarrage

1. Ouvrir `SystemePlacement.sln` dans Visual Studio.
2. Configurer la connexion MySQL dans `SystemePlacement.Web/appsettings.json`.
3. Définir `SystemePlacement.Web` comme projet de démarrage.
4. Lancer l’application.

Pour appliquer les migrations depuis un terminal :

```bash
dotnet ef database update --project SystemePlacement.Web
