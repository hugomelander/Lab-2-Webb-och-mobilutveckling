using Microsoft.Extensions.Logging;
using ConcertBooking.Mobile.Services;
using ConcertBooking.Mobile.ViewModels;
using ConcertBooking.Mobile.Views;


namespace ConcertBooking.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl)
            });

            builder.Services.AddSingleton<ConcertApi>();
            builder.Services.AddSingleton<ConcertsViewModel>();
            builder.Services.AddSingleton<ConcertsPage>();

#endif

            return builder.Build();
        }
    }
}
