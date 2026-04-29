using GestionTaches.ViewModels;

namespace GestionTaches;

public partial class MainPage : ContentPage
{
    private readonly TachesViewModel _viewModel;
    private bool _isInitialized;
    private readonly bool _xamlLoaded;

    public MainPage(TachesViewModel viewModel)
    {
        _viewModel = viewModel;

        try
        {
            InitializeComponent();
            BindingContext = viewModel;
            _xamlLoaded = true;
        }
        catch (Exception ex)
        {
            _xamlLoaded = false;
            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = 16,
                    Spacing = 8,
                    Children =
                    {
                        new Label
                        {
                            Text = "Erreur de chargement XAML de MainPage",
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Colors.Red
                        },
                        new Label
                        {
                            Text = ex.ToString(),
                            FontSize = 12
                        }
                    }
                }
            };
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_isInitialized)
        {
            return;
        }

        if (!_xamlLoaded)
        {
            return;
        }

        _isInitialized = true;
        try
        {
            await _viewModel.InitialiserAsync();
        }
        catch (Exception ex)
        {
            _viewModel.MessageColor = "#BE123C";
            _viewModel.Message = $"Erreur au demarrage: {ex.Message}";
        }
    }
}