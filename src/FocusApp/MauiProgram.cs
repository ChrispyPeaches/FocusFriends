using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.Sample;
using FocusApp.Pages;
using Microsoft.Extensions.Logging;

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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // App Shell
            builder.Services.AddTransient<AppShell>();

            builder.Services
                .AddSingleton<App>();

            builder.Services.AddTransient<MainPage, MainViewModel>();

            // C# Hot Reload Handler
            builder.Services.AddSingleton<ICommunityToolkitHotReloadHandler, HotReloadHandler>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}