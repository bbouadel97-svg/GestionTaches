using System.Windows;
using GestionTaches.Data;
using GestionTaches.Models;
using GestionTaches.Services;
using Microsoft.EntityFrameworkCore;
using System.Windows.Threading;

namespace GestionTaches;

public partial class MainWindow : Window
{
    private readonly ApplicationDbContext _dbContext;
    private readonly TacheService _tacheService;
    private readonly DispatcherTimer _clockTimer = new() { Interval = TimeSpan.FromSeconds(1) };
    private List<TacheItem> _taches = [];
    private Guid? _selectedId;

    public MainWindow()
    {
        InitializeComponent();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=gestiontaches.db")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _dbContext.Database.EnsureCreated();

        _tacheService = new TacheService(_dbContext);
        _clockTimer.Tick += (_, _) => MettreAJourHorloge();
        _clockTimer.Start();
        MettreAJourHorloge();

        _ = InitialiserAsync();
    }

    private async Task InitialiserAsync()
    {
        await _tacheService.SeedDataAsync();

        PrioriteComboBox.ItemsSource = Enum.GetValues<PrioriteTache>();
        PrioriteComboBox.SelectedItem = PrioriteTache.Normale;

        StatutComboBox.ItemsSource = Enum.GetValues<StatutTache>();
        StatutComboBox.SelectedItem = StatutTache.AFaire;

        FiltreStatutComboBox.Items.Add("Tous les statuts");
        foreach (var statut in Enum.GetValues<StatutTache>())
        {
            FiltreStatutComboBox.Items.Add(statut);
        }

        FiltreStatutComboBox.SelectedIndex = 0;
        await RafraichirAsync();
    }

    private async Task RafraichirAsync()
    {
        _taches = (await _tacheService.GetAllAsync()).ToList();
        AppliquerFiltres();
    }

    private void AppliquerFiltres()
    {
        var recherche = RechercheTextBox.Text?.Trim() ?? string.Empty;
        var filtre = FiltreStatutComboBox.SelectedItem;

        IEnumerable<TacheItem> query = _taches;

        if (!string.IsNullOrWhiteSpace(recherche))
        {
            query = query.Where(t => t.Titre.Contains(recherche, StringComparison.OrdinalIgnoreCase));
        }

        if (filtre is StatutTache statut)
        {
            query = query.Where(t => t.Statut == statut);
        }

        var resultats = query.ToList();
        TachesDataGrid.ItemsSource = resultats;
        ResultatsTextBlock.Text = $"{resultats.Count} resultat(s)";

        MettreAJourIndicateurs();
    }

    private async void Ajouter_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitreTextBox.Text))
        {
            MessageTextBlock.Text = "Le titre est obligatoire.";
            return;
        }

        var tache = new TacheItem
        {
            Titre = TitreTextBox.Text.Trim(),
            Description = DescriptionTextBox.Text.Trim(),
            Assignee = AssigneeTextBox.Text.Trim(),
            Priorite = (PrioriteTache)(PrioriteComboBox.SelectedItem ?? PrioriteTache.Normale),
            Statut = (StatutTache)(StatutComboBox.SelectedItem ?? StatutTache.AFaire),
            DateEcheance = DateEcheancePicker.SelectedDate.HasValue
                ? DateOnly.FromDateTime(DateEcheancePicker.SelectedDate.Value)
                : null
        };

        await _tacheService.AjouterAsync(tache);
        await RafraichirAsync();
        ViderFormulaire();
        MessageTextBlock.Text = "Tache ajoutee.";
    }

    private async void Modifier_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedId is null)
        {
            MessageTextBlock.Text = "Selectionnez une tache.";
            return;
        }

        var tache = new TacheItem
        {
            Id = _selectedId.Value,
            Titre = TitreTextBox.Text.Trim(),
            Description = DescriptionTextBox.Text.Trim(),
            Assignee = AssigneeTextBox.Text.Trim(),
            Priorite = (PrioriteTache)(PrioriteComboBox.SelectedItem ?? PrioriteTache.Normale),
            Statut = (StatutTache)(StatutComboBox.SelectedItem ?? StatutTache.AFaire),
            DateEcheance = DateEcheancePicker.SelectedDate.HasValue
                ? DateOnly.FromDateTime(DateEcheancePicker.SelectedDate.Value)
                : null
        };

        var ok = await _tacheService.ModifierAsync(tache);
        MessageTextBlock.Text = ok ? "Tache modifiee." : "Tache introuvable.";

        await RafraichirAsync();
    }

    private async void Supprimer_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedId is null)
        {
            MessageTextBlock.Text = "Selectionnez une tache.";
            return;
        }

        var ok = await _tacheService.SupprimerAsync(_selectedId.Value);
        MessageTextBlock.Text = ok ? "Tache supprimee." : "Tache introuvable.";

        _selectedId = null;
        await RafraichirAsync();
        ViderFormulaire();
    }

    private void TachesDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (TachesDataGrid.SelectedItem is not TacheItem item)
        {
            return;
        }

        _selectedId = item.Id;
        TitreTextBox.Text = item.Titre;
        DescriptionTextBox.Text = item.Description;
        AssigneeTextBox.Text = item.Assignee;
        PrioriteComboBox.SelectedItem = item.Priorite;
        StatutComboBox.SelectedItem = item.Statut;
        DateEcheancePicker.SelectedDate = item.DateEcheance?.ToDateTime(TimeOnly.MinValue);
    }

    private void Filtres_Changed(object sender, RoutedEventArgs e)
    {
        AppliquerFiltres();
    }

    private void ViderFormulaire()
    {
        TitreTextBox.Text = string.Empty;
        DescriptionTextBox.Text = string.Empty;
        AssigneeTextBox.Text = string.Empty;
        PrioriteComboBox.SelectedItem = PrioriteTache.Normale;
        StatutComboBox.SelectedItem = StatutTache.AFaire;
        DateEcheancePicker.SelectedDate = null;
    }

    private void MettreAJourIndicateurs()
    {
        TotalTextBlock.Text = _taches.Count.ToString();
        AFaireTextBlock.Text = _taches.Count(t => t.Statut == StatutTache.AFaire).ToString();
        EnCoursTextBlock.Text = _taches.Count(t => t.Statut == StatutTache.EnCours).ToString();
        BloqueesTextBlock.Text = _taches.Count(t => t.Statut == StatutTache.Bloquee).ToString();
    }

    private void MettreAJourHorloge()
    {
        HorlogeTextBlock.Text = DateTime.Now.ToString("dddd dd MMM yyyy - HH:mm:ss");
    }

    protected override void OnClosed(EventArgs e)
    {
        _clockTimer.Stop();
        _dbContext.Dispose();
        base.OnClosed(e);
    }
}
