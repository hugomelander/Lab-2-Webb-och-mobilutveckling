using ConcertBooking.Mobile.Services;
using ConcertBooking.Mobile.ViewModels;
using ConcertBooking.Mobile.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace ConcertBooking.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // ----------- HttpClient -----------
        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri(ApiConfig.BaseUrl)
        });

        // ----------- Services -----------
        builder.Services.AddSingleton<ConcertApi>();
        builder.Services.AddSingleton<IDialogService, DialogService>();


        // ----------- ViewModels -----------
        builder.Services.AddSingleton<ConcertsViewModel>();
        builder.Services.AddSingleton<PerformancesViewModel>();

        // ----------- Views (Pages) -----------
        builder.Services.AddSingleton<ConcertsPage>();
        builder.Services.AddSingleton<PerformancesPage>();

        return builder.Build();
    }
}
