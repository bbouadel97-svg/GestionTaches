using GestionTaches.Data;
using GestionTaches.Services;
using GestionTaches.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GestionTaches;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "gestiontaches.db");

        builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlite($"Data Source={databasePath}"));
        builder.Services.AddSingleton<TacheService>();
        builder.Services.AddSingleton<TachesViewModel>();
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}