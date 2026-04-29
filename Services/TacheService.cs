using GestionTaches.Data;
using GestionTaches.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionTaches.Services;

public sealed class TacheService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<IReadOnlyList<TacheItem>> GetAllAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.Taches
            .OrderBy(t => t.Statut)
            .ThenByDescending(t => t.Priorite)
            .ThenBy(t => t.DateEcheance)
            .ThenBy(t => t.DateCreation)
            .ToListAsync();
    }

    public async Task<TacheItem?> GetByIdAsync(Guid id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        return await dbContext.Taches.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TacheItem> AjouterAsync(TacheItem nouvelleTache)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var tacheAAjouter = new TacheItem
        {
            Id = Guid.NewGuid(),
            Titre = nouvelleTache.Titre,
            Description = nouvelleTache.Description,
            Assignee = nouvelleTache.Assignee,
            Priorite = nouvelleTache.Priorite,
            Statut = nouvelleTache.Statut,
            DateEcheance = nouvelleTache.DateEcheance,
            DateCreation = DateTime.UtcNow
        };

        dbContext.Taches.Add(tacheAAjouter);
        await dbContext.SaveChangesAsync();
        return tacheAAjouter;
    }

    public async Task<bool> ModifierAsync(TacheItem tacheMaj)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var existante = await dbContext.Taches.FirstOrDefaultAsync(t => t.Id == tacheMaj.Id);
        if (existante is null)
        {
            return false;
        }

        existante.Titre = tacheMaj.Titre;
        existante.Description = tacheMaj.Description;
        existante.Assignee = tacheMaj.Assignee;
        existante.Priorite = tacheMaj.Priorite;
        existante.Statut = tacheMaj.Statut;
        existante.DateEcheance = tacheMaj.DateEcheance;
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SupprimerAsync(Guid id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var existante = await dbContext.Taches.FirstOrDefaultAsync(t => t.Id == id);
        if (existante is null)
        {
            return false;
        }

        dbContext.Taches.Remove(existante);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangerStatutAsync(Guid id, StatutTache nouveauStatut)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var existante = await dbContext.Taches.FirstOrDefaultAsync(t => t.Id == id);
        if (existante is null)
        {
            return false;
        }

        existante.Statut = nouveauStatut;
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task SeedDataAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        // Ensure the SQLite schema exists before executing queries.
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Taches.AnyAsync())
        {
            return;
        }

        dbContext.Taches.AddRange(
            new TacheItem
            {
                Titre = "Preparer la reunion hebdomadaire",
                Description = "Rassembler les indicateurs de la semaine et les points de blocage.",
                Assignee = "Nadia",
                Priorite = PrioriteTache.Haute,
                Statut = StatutTache.EnCours,
                DateEcheance = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                DateCreation = DateTime.UtcNow
            },
            new TacheItem
            {
                Titre = "Mettre a jour la liste clients",
                Description = "Verifier les nouveaux contacts et completer les informations manquantes.",
                Assignee = "Karim",
                Priorite = PrioriteTache.Normale,
                Statut = StatutTache.AFaire,
                DateEcheance = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
                DateCreation = DateTime.UtcNow
            });

        await dbContext.SaveChangesAsync();
    }
}