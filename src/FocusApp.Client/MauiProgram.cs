using System.Reflection;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Clients;
using FocusApp.Shared.Data;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;
using SimpleToolkit.SimpleShell;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using FocusApp.Client.Views.Shop;
using FocusApp.Client.Views.Social;

namespace FocusApp.Client
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
                .RegisterDatabaseContext()
                .RegisterRefitClient()
                .RegisterServices()
                .RegisterPages()
                .RegisterPopups();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static IServiceCollection RegisterDatabaseContext(this IServiceCollection services)
        {
            var migrationAssembly = typeof(FocusAppContext).Assembly.GetName().Name;
            services.AddDbContext<IFocusAppContext, FocusAppContext>(opts =>
            {
                opts.UseSqlite($"Filename={Consts.DatabasePath}", x => x.MigrationsAssembly(migrationAssembly));
            });

            return services;
        }

        private static IServiceCollection RegisterRefitClient(this IServiceCollection services)
        {
            services
                .AddRefitClient<IAPIClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://10.0.2.2:5223"));

            return services;
        }

        private static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Registered as a singleton so the timer is not reset by page navigation
            services.AddSingleton<ITimerService, TimerService>();
            services.AddSingleton<Helpers.PopupService>();

            return services;
        }

        private static IServiceCollection RegisterPages(this IServiceCollection services)
        {
            IEnumerable<Type>? pageTypes = Assembly.GetAssembly(typeof(BasePage))?
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BasePage)));

            if (pageTypes is not null)
            {
                foreach (Type pageType in pageTypes)
                {
                    services.AddTransient(pageType);
                }
            }

            return services;
        }

        private static IServiceCollection RegisterPopups(this IServiceCollection services)
        {
            IEnumerable<Type>? popupTypes = Assembly.GetAssembly(typeof(BasePopup))?
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BasePopup)));

            if (popupTypes is not null)
            {
                foreach (Type popupType in popupTypes)
                {
                    services.AddTransient(popupType);
                }
            }

            return services;
        }
    }
}