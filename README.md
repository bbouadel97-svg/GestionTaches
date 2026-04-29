# GestionTaches

Application de gestion de taches developpee en .NET MAUI avec architecture MVVM et persistance SQLite via Entity Framework Core.

## Livrables Techno Boost

Ce depot contient maintenant les documents principaux attendus pour le projet :

- `README.md` : presentation technique du projet
- `CHOIX_TECHNOLOGIE.md` : justification du choix technologique
- `CAHIER_DES_CHARGES.md` : objectifs, perimetre et livrables du projet

## Apercu

Le projet permet de :

- creer une tache
- modifier une tache existante
- supprimer une tache
- filtrer les taches par texte, statut, priorite et assignee
- afficher les taches en liste detaillee
- afficher les taches en vue Kanban
- suivre quelques indicateurs simples (total, a faire, en cours, bloquees)
- reutiliser une couche de service isolee de l'interface

Au demarrage, l'application cree la base locale si necessaire et ajoute un petit jeu de donnees initial si aucune tache n'existe.

## Stack technique

- .NET 10 for Windows (`net10.0-windows10.0.19041.0`)
- .NET MAUI pour l'interface graphique desktop
- MVVM avec `CommunityToolkit.Mvvm`
- Entity Framework Core 10
- SQLite pour le stockage local
- C# avec nullable reference types activees

## Prerequis

- Windows
- SDK .NET 10 installe

## Lancer l'application

Depuis le dossier du projet :

```powershell
dotnet run --project .\GestionTaches.csproj
```

Depuis n'importe quel dossier :

```powershell
dotnet run --project "c:\Users\User\OneDrive\Bureau\Mes Projets 2eme_Sem\GestionTaches\GestionTaches.csproj"
```

Pour arreter l'application dans le terminal :

```powershell
Ctrl+C
```

## Structure du projet

- `GestionTaches.csproj` : fichier projet principal
- `App.xaml` et `App.xaml.cs` : ressources globales et creation de la fenetre MAUI
- `MauiProgram.cs` : bootstrap MAUI et injection des dependances
- `MainPage.xaml` et `MainPage.xaml.cs` : page principale MAUI
- `ViewModels/TachesViewModel.cs` : etat de l'ecran, commandes et filtres MVVM
- `Data/ApplicationDbContext.cs` : contexte Entity Framework Core
- `Models/TacheItem.cs` : modele de tache et enums de priorite/statut
- `Services/TacheService.cs` : operations CRUD et initialisation des donnees
- `Platforms/Windows/` : fichiers Windows specifiques MAUI

## Base de donnees

L'application utilise une base SQLite locale :

- fichier : `gestiontaches.db`
- creation automatique au premier lancement via EF Core
- emplacement runtime : dossier applicatif local de MAUI (`FileSystem.AppDataDirectory`)

La table principale geree par l'application correspond au modele `TacheItem`.

## Modele de donnees

Une tache contient notamment :

- `Id`
- `Titre`
- `Description`
- `Assignee`
- `Priorite`
- `Statut`
- `DateEcheance`
- `DateCreation`

### Priorites disponibles

- `Basse`
- `Normale`
- `Haute`
- `Critique`

### Statuts disponibles

- `AFaire`
- `EnCours`
- `Bloquee`
- `Terminee`

## Fonctionnement principal

Au lancement de l'application :

1. `MauiProgram` configure les dependances et la factory de `ApplicationDbContext`
2. le `TacheService` initialise la base si necessaire et injecte des donnees de demonstration si elle est vide
3. `TachesViewModel` charge les listes de priorite, statut et filtres
4. les taches sont projetees dans la vue liste et la vue Kanban
5. les commandes MVVM gerent l'ajout, la modification, la suppression et le rafraichissement

## Points techniques utiles

- les taches sont triees par statut, puis priorite descendante, puis echeance, puis date de creation
- le titre est obligatoire
- certaines longueurs sont limitees par le modele EF Core et les annotations de validation
- la logique UI n'est plus dans le code-behind metier, elle est centralisee dans le ViewModel
- le contexte EF est cree a la demande via `IDbContextFactory` pour eviter les couplages de cycle de vie avec l'UI

## Packages utilises

Principales dependances declarees dans le projet :

- `CommunityToolkit.Mvvm`
- `Microsoft.EntityFrameworkCore.Sqlite`
- `Microsoft.Maui.Controls`
- `Microsoft.Extensions.Logging.Debug`

## Remarques

- le projet cible actuellement Windows via MAUI, ce qui permet une validation locale immediate dans cet environnement


## Pour lancer le projet
- ddotnet run --project GestionTaches.csproj -f net10.0-windows10.0.19041.0