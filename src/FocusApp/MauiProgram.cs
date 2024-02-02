using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using FocusApp.Clients;
using FocusApp.Helpers;
using FocusApp.Resources.FontAwesomeIcons;
using Microsoft.Extensions.Logging;
using Refit;
using SimpleToolkit.SimpleShell;

namespace FocusApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMarkup()
                .UseSimpleShell()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Font-Awesome-6-Free-Solid.otf", nameof(SolidIcons));
                    fonts.AddFont("Font-Awesome-6-Free-Regular.otf", nameof(LineArtIcons));
                });

            builder.Services
                .AddRefitClient<IAPIClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://10.0.2.2:5223"));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}