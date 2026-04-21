# GestionTaches

Application de gestion de taches developpee en WPF avec .NET et Entity Framework Core.

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
- afficher les taches en tableau
- afficher les taches en vue Kanban
- suivre quelques indicateurs simples (total, a faire, en cours, bloquees)

Au demarrage, l'application cree la base locale si necessaire et ajoute un petit jeu de donnees initial si aucune tache n'existe.

## Stack technique

- .NET 10 for Windows (`net10.0-windows`)
- WPF pour l'interface graphique
- Entity Framework Core 10
- SQLite pour le stockage local
- C# avec nullable reference types activees

## Prerequis

- Windows
- SDK .NET 10 installe

## Lancer l'application

Depuis le dossier du projet :

```powershell
dotnet run
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
- `App.xaml` et `App.xaml.cs` : point d'entree WPF
- `MainWindow.xaml` et `MainWindow.xaml.cs` : interface principale et logique UI
- `Data/ApplicationDbContext.cs` : contexte Entity Framework Core
- `Models/TacheItem.cs` : modele de tache et enums de priorite/statut
- `Services/TacheService.cs` : operations CRUD et initialisation des donnees
- `Properties/launchSettings.json` : profils de lancement
- `wwwroot/` : ressources statiques presentes dans le projet

## Base de donnees

L'application utilise une base SQLite locale :

- fichier : `gestiontaches.db`
- creation automatique au demarrage avec `Database.EnsureCreated()`

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

Au lancement de la fenetre principale :

1. la connexion SQLite locale est configuree
2. la base est creee si elle n'existe pas
3. des donnees de demonstration sont ajoutees si la table est vide
4. les listes de priorite et de statut sont chargees
5. les taches sont affichees, filtrees et reparties dans la vue Kanban

## Points techniques utiles

- les taches sont triees par statut, puis priorite descendante, puis echeance, puis date de creation
- le titre est obligatoire
- certaines longueurs sont limitees par le modele EF Core et les annotations de validation
- l'horloge de l'interface est mise a jour toutes les secondes

## Packages utilises

Principales dependances declarees dans le projet :

- `Microsoft.EntityFrameworkCore.Design`
- `Microsoft.EntityFrameworkCore.Sqlite`
- `Microsoft.Build`
- `Microsoft.Build.Tasks.Core`

## Remarques

- le dossier `Pages/` est present mais vide a ce stade
- le fichier `launchSettings.json` contient des profils `http` et `https`, mais l'application actuelle est une application WPF bureau


## Pour lancer le projet
- dotnet run --project "c:\Users\User\OneDrive\Bureau\Mes Projets 2eme_Sem\GestionTaches\GestionTaches.csproj"
