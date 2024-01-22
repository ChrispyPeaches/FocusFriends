using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using FocusApp.Resources.FontAwesomeIcons;
using Microsoft.Extensions.Logging;
using Sharpnado.Tabs;

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
                .UseSharpnadoTabs(loggerEnable: false)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Font-Awesome-6-Free-Solid.otf", nameof(SolidIcons));
                    fonts.AddFont("Font-Awesome-6-Free-Regular.otf", nameof(LineArtIcons));
                });

            builder.Services.AddSingleton<IPopupService, PopupService>();
            builder.Services.AddSingleton<MainPage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}