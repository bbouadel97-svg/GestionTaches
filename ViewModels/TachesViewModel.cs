using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GestionTaches.Models;
using GestionTaches.Services;

namespace GestionTaches.ViewModels;

public partial class TachesViewModel(TacheService tacheService) : ObservableObject
{
    private const string TousLesStatuts = "Tous les statuts";
    private const string ToutesLesPriorites = "Toutes les priorites";
    private List<TacheItem> _toutesLesTaches = [];
    private bool _initialise;
    private string _titre = string.Empty;
    private string _description = string.Empty;
    private string _assignee = string.Empty;
    private PrioriteTache _prioriteSelectionnee = PrioriteTache.Normale;
    private StatutTache _statutSelectionne = StatutTache.AFaire;
    private bool _aUneEcheance;
    private DateTime _dateEcheance = DateTime.Today;
    private string _recherche = string.Empty;
    private string _filtreStatutSelectionne = TousLesStatuts;
    private string _filtrePrioriteSelectionnee = ToutesLesPriorites;
    private string _filtreAssignee = string.Empty;
    private TacheItem? _tacheSelectionnee;
    private string _message = string.Empty;
    private string _messageColor = "#0F766E";
    private bool _isBusy;
    private int _totalTaches;
    private int _totalAFaire;
    private int _totalEnCours;
    private int _totalBloquees;
    private string _resultatsLabel = "0 resultat(s)";
    private string _derniereMiseAJourLabel = "Derniere mise a jour: jamais";

    public ObservableCollection<TacheItem> TachesFiltrees { get; } = [];
    public ObservableCollection<TacheItem> TachesAFaire { get; } = [];
    public ObservableCollection<TacheItem> TachesEnCours { get; } = [];
    public ObservableCollection<TacheItem> TachesBloquees { get; } = [];
    public ObservableCollection<TacheItem> TachesTerminees { get; } = [];

    public IReadOnlyList<PrioriteTache> Priorites { get; } = Enum.GetValues<PrioriteTache>();
    public IReadOnlyList<StatutTache> Statuts { get; } = Enum.GetValues<StatutTache>();
    public IReadOnlyList<string> FiltresStatut { get; } = [TousLesStatuts, .. Enum.GetNames<StatutTache>()];
    public IReadOnlyList<string> FiltresPriorite { get; } = [ToutesLesPriorites, .. Enum.GetNames<PrioriteTache>()];

    public string Titre
    {
        get => _titre;
        set => SetProperty(ref _titre, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string Assignee
    {
        get => _assignee;
        set => SetProperty(ref _assignee, value);
    }

    public PrioriteTache PrioriteSelectionnee
    {
        get => _prioriteSelectionnee;
        set => SetProperty(ref _prioriteSelectionnee, value);
    }

    public StatutTache StatutSelectionne
    {
        get => _statutSelectionne;
        set => SetProperty(ref _statutSelectionne, value);
    }

    public bool AUneEcheance
    {
        get => _aUneEcheance;
        set => SetProperty(ref _aUneEcheance, value);
    }

    public DateTime DateEcheance
    {
        get => _dateEcheance;
        set => SetProperty(ref _dateEcheance, value);
    }

    public string Recherche
    {
        get => _recherche;
        set
        {
            if (SetProperty(ref _recherche, value))
            {
                AppliquerFiltres();
            }
        }
    }

    public string FiltreStatutSelectionne
    {
        get => _filtreStatutSelectionne;
        set
        {
            if (SetProperty(ref _filtreStatutSelectionne, value))
            {
                AppliquerFiltres();
            }
        }
    }

    public string FiltrePrioriteSelectionnee
    {
        get => _filtrePrioriteSelectionnee;
        set
        {
            if (SetProperty(ref _filtrePrioriteSelectionnee, value))
            {
                AppliquerFiltres();
            }
        }
    }

    public string FiltreAssignee
    {
        get => _filtreAssignee;
        set
        {
            if (SetProperty(ref _filtreAssignee, value))
            {
                AppliquerFiltres();
            }
        }
    }

    public TacheItem? TacheSelectionnee
    {
        get => _tacheSelectionnee;
        set
        {
            if (!SetProperty(ref _tacheSelectionnee, value))
            {
                return;
            }

            if (value is not null)
            {
                Titre = value.Titre;
                Description = value.Description;
                Assignee = value.Assignee;
                PrioriteSelectionnee = value.Priorite;
                StatutSelectionne = value.Statut;
                AUneEcheance = value.DateEcheance.HasValue;
                DateEcheance = value.DateEcheance?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Today;
                Message = string.Empty;
            }

            ModifierCommand.NotifyCanExecuteChanged();
            SupprimerCommand.NotifyCanExecuteChanged();
        }
    }

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public string MessageColor
    {
        get => _messageColor;
        set => SetProperty(ref _messageColor, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public int TotalTaches
    {
        get => _totalTaches;
        set => SetProperty(ref _totalTaches, value);
    }

    public int TotalAFaire
    {
        get => _totalAFaire;
        set => SetProperty(ref _totalAFaire, value);
    }

    public int TotalEnCours
    {
        get => _totalEnCours;
        set => SetProperty(ref _totalEnCours, value);
    }

    public int TotalBloquees
    {
        get => _totalBloquees;
        set => SetProperty(ref _totalBloquees, value);
    }

    public string ResultatsLabel
    {
        get => _resultatsLabel;
        set => SetProperty(ref _resultatsLabel, value);
    }

    public string DerniereMiseAJourLabel
    {
        get => _derniereMiseAJourLabel;
        set => SetProperty(ref _derniereMiseAJourLabel, value);
    }

    public async Task InitialiserAsync()
    {
        if (_initialise)
        {
            return;
        }

        _initialise = true;
        await tacheService.SeedDataAsync();
        await RafraichirAsync();
    }

    [RelayCommand]
    private async Task RafraichirAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            _toutesLesTaches = (await tacheService.GetAllAsync()).ToList();
            TotalTaches = _toutesLesTaches.Count;
            TotalAFaire = _toutesLesTaches.Count(t => t.Statut == StatutTache.AFaire);
            TotalEnCours = _toutesLesTaches.Count(t => t.Statut == StatutTache.EnCours);
            TotalBloquees = _toutesLesTaches.Count(t => t.Statut == StatutTache.Bloquee);
            DerniereMiseAJourLabel = $"Derniere mise a jour: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            AppliquerFiltres();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AjouterAsync()
    {
        if (!ValiderFormulaire())
        {
            return;
        }

        var tache = ConstruireTache();
        await tacheService.AjouterAsync(tache);
        DefinirMessage("Tache ajoutee.", false);
        await RafraichirAsync();
        Vider();
    }

    private bool PeutModifierOuSupprimer() => TacheSelectionnee is not null;

    [RelayCommand(CanExecute = nameof(PeutModifierOuSupprimer))]
    private async Task ModifierAsync()
    {
        if (TacheSelectionnee is null || !ValiderFormulaire())
        {
            return;
        }

        var tache = ConstruireTache();
        tache.Id = TacheSelectionnee.Id;

        var ok = await tacheService.ModifierAsync(tache);
        DefinirMessage(ok ? "Tache modifiee." : "Tache introuvable.", !ok);
        await RafraichirAsync();
    }

    [RelayCommand(CanExecute = nameof(PeutModifierOuSupprimer))]
    private async Task SupprimerAsync()
    {
        if (TacheSelectionnee is null)
        {
            return;
        }

        var ok = await tacheService.SupprimerAsync(TacheSelectionnee.Id);
        DefinirMessage(ok ? "Tache supprimee." : "Tache introuvable.", !ok);
        await RafraichirAsync();
        Vider();
    }

    [RelayCommand]
    private void Vider() => ReinitialiserFormulaire();

    private bool ValiderFormulaire()
    {
        if (string.IsNullOrWhiteSpace(Titre))
        {
            DefinirMessage("Le titre est obligatoire.", true);
            return false;
        }

        if (Titre.Length > 120)
        {
            DefinirMessage("Le titre ne peut pas depasser 120 caracteres.", true);
            return false;
        }

        if (Description.Length > 400)
        {
            DefinirMessage("La description ne peut pas depasser 400 caracteres.", true);
            return false;
        }

        if (Assignee.Length > 80)
        {
            DefinirMessage("Le nom de l'assigne ne peut pas depasser 80 caracteres.", true);
            return false;
        }

        return true;
    }

    private TacheItem ConstruireTache() => new()
    {
        Titre = Titre.Trim(),
        Description = Description.Trim(),
        Assignee = Assignee.Trim(),
        Priorite = PrioriteSelectionnee,
        Statut = StatutSelectionne,
        DateEcheance = AUneEcheance ? DateOnly.FromDateTime(DateEcheance) : null
    };

    private void AppliquerFiltres()
    {
        IEnumerable<TacheItem> query = _toutesLesTaches;

        if (!string.IsNullOrWhiteSpace(Recherche))
        {
            query = query.Where(t =>
                t.Titre.Contains(Recherche, StringComparison.OrdinalIgnoreCase)
                || t.Description.Contains(Recherche, StringComparison.OrdinalIgnoreCase)
                || t.Assignee.Contains(Recherche, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.Equals(FiltreStatutSelectionne, TousLesStatuts, StringComparison.Ordinal) && Enum.TryParse<StatutTache>(FiltreStatutSelectionne, out var statut))
        {
            query = query.Where(t => t.Statut == statut);
        }

        if (!string.Equals(FiltrePrioriteSelectionnee, ToutesLesPriorites, StringComparison.Ordinal) && Enum.TryParse<PrioriteTache>(FiltrePrioriteSelectionnee, out var priorite))
        {
            query = query.Where(t => t.Priorite == priorite);
        }

        if (!string.IsNullOrWhiteSpace(FiltreAssignee))
        {
            query = query.Where(t => t.Assignee.Contains(FiltreAssignee, StringComparison.OrdinalIgnoreCase));
        }

        var resultats = query.ToList();
        RemplirCollection(TachesFiltrees, resultats);
        RemplirCollection(TachesAFaire, resultats.Where(t => t.Statut == StatutTache.AFaire));
        RemplirCollection(TachesEnCours, resultats.Where(t => t.Statut == StatutTache.EnCours));
        RemplirCollection(TachesBloquees, resultats.Where(t => t.Statut == StatutTache.Bloquee));
        RemplirCollection(TachesTerminees, resultats.Where(t => t.Statut == StatutTache.Terminee));
        ResultatsLabel = $"{resultats.Count} resultat(s)";
    }

    private static void RemplirCollection(ObservableCollection<TacheItem> collection, IEnumerable<TacheItem> items)
    {
        collection.Clear();
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    private void ReinitialiserFormulaire()
    {
        TacheSelectionnee = null;
        Titre = string.Empty;
        Description = string.Empty;
        Assignee = string.Empty;
        PrioriteSelectionnee = PrioriteTache.Normale;
        StatutSelectionne = StatutTache.AFaire;
        AUneEcheance = false;
        DateEcheance = DateTime.Today;
        Message = string.Empty;

        ModifierCommand.NotifyCanExecuteChanged();
        SupprimerCommand.NotifyCanExecuteChanged();
    }

    private void DefinirMessage(string texte, bool estErreur)
    {
        Message = texte;
        MessageColor = estErreur ? "#BE123C" : "#0F766E";
    }
}