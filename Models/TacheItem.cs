using System.ComponentModel.DataAnnotations;

namespace GestionTaches.Models;

public enum PrioriteTache
{
    Basse,
    Normale,
    Haute,
    Critique
}

public enum StatutTache
{
    AFaire,
    EnCours,
    Bloquee,
    Terminee
}

public sealed class TacheItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(120, ErrorMessage = "Le titre ne peut pas depasser 120 caracteres.")]
    public string Titre { get; set; } = string.Empty;

    [StringLength(400, ErrorMessage = "La description ne peut pas depasser 400 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [StringLength(80, ErrorMessage = "Le nom de l'assigne est trop long.")]
    public string Assignee { get; set; } = string.Empty;
    public PrioriteTache Priorite { get; set; } = PrioriteTache.Normale;
    public StatutTache Statut { get; set; } = StatutTache.AFaire;
    public DateOnly? DateEcheance { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}