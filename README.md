# Système de placement — Backend

API REST ASP.NET Core d’une plateforme de placement développée en équipe dans le cadre de mon stage en développement full-stack au Cégep Gérald-Godin en 2026.

Le backend centralise l’authentification, les règles métier et l’accès aux données de la plateforme de placement utilisée par les étudiants, les employeurs, les administrateurs et les responsables de stage.

## Aperçu du projet

L’API prend en charge plusieurs fonctionnalités du processus de placement, notamment :

- la gestion des utilisateurs et des rôles ;
- la gestion des cégeps et des domaines d’études ;
- les profils d’entreprise ;
- les offres d’emploi et de stage ;
- les candidatures ;
- les demandes de stage ;
- les offres de stage directes ;
- les confirmations d’emploi et de stage ;
- les recommandations ;
- les notifications ;
- la génération de documents PDF.

L’API applique également des règles d’autorisation et de contrôle d’accès selon le rôle et le contexte de l’utilisateur.

## Mes contributions

Ce projet a été réalisé en équipe. J’ai principalement contribué au backend à travers ma branche `Dev2-Jeff` et plusieurs pull requests intégrées au projet collaboratif.

Parmi mes principales contributions :

- développement des API CRUD pour les cégeps et les domaines d’études ;
- développement de l’API de gestion du profil d’entreprise ;
- ajout et amélioration de fonctionnalités liées aux candidatures ;
- développement du processus d’offres de stage directes ;
- ajout du processus de confirmation d’emploi ;
- synchronisation des candidatures avec les offres directes acceptées ;
- amélioration des règles de validation des candidatures ;
- sécurisation des routes selon les rôles utilisateurs ;
- ajout de contrôles d’autorisation et de propriété des ressources ;
- renforcement de l’isolation des données entre employeurs et étudiants ;
- sécurisation des recommandations et de leur accès ;
- amélioration du système de notifications ;
- prévention de certaines opérations ou offres en double ;
- validation de l’identité des domaines dans le contexte multi-cégeps ;
- blocage des candidatures vers des offres inactives ;
- génération de véritables documents PDF pour les offres ;
- ajout et amélioration de tests de régression liés à la sécurité et aux règles métier ;
- correction de problèmes d’encodage et de textes en français ;
- participation aux phases de QA, de correction de bogues et de stabilisation de l’API.

Au total, j’ai soumis 30 pull requests au dépôt backend collaboratif durant le projet.

## Technologies

- C#
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- MySQL
- Authentification JWT
- API REST
- Swagger / OpenAPI

## Architecture

Ce dépôt contient le backend de la plateforme.

Le frontend React est disponible ici :

[placement-platform-frontend](https://github.com/jeffsawma/placement-platform-frontend)

## Démarrage

1. Ouvrir `SystemePlacement.sln` dans Visual Studio.
2. Configurer la chaîne de connexion MySQL et la clé JWT avec des variables d’environnement ou une configuration locale non suivie par Git.
3. Définir `SystemePlacement.Web` comme projet de démarrage.
4. Lancer l’application.

Pour appliquer les migrations Entity Framework Core depuis un terminal :

```bash
dotnet ef database update --project SystemePlacement.Web
```

## Configuration

Les valeurs sensibles ne sont pas stockées dans `appsettings.json`.

La chaîne de connexion et la clé JWT peuvent être fournies avec les variables d’environnement `ConnectionStrings__DefaultConnection` et `Jwt__Key`.

Les informations sensibles ou propres à un environnement local ne doivent pas être ajoutées au dépôt public.

## Collaboration

Ce projet a été développé dans le cadre d’un stage collaboratif au Cégep Gérald-Godin.

L’historique Git original a été conservé afin de refléter le travail réalisé par l’ensemble de l’équipe ainsi que mes propres contributions.

Dépôt collaboratif original :

[aichamayaa/BACKEND-Stage2026](https://github.com/aichamayaa/BACKEND-Stage2026)
