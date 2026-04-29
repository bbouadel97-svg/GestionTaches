using GestionTaches.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionTaches.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TacheItem> Taches => Set<TacheItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TacheItem>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Titre).IsRequired().HasMaxLength(120);
            entity.Property(t => t.Description).HasMaxLength(400);
            entity.Property(t => t.Assignee).HasMaxLength(80);
            entity.Property(t => t.Priorite).HasConversion<string>();
            entity.Property(t => t.Statut).HasConversion<string>();
            entity.Property(t => t.DateCreation).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
