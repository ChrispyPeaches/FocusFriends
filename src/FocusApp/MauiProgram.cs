using System.Reflection;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using FocusApp.Clients;
using FocusAppShared.Data;
using FocusApp.Helpers;
using FocusApp.Resources;
using FocusApp.Resources.FontAwesomeIcons;
using FocusApp.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;
using SimpleToolkit.SimpleShell;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using FocusApp.Views.Shop;

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
            

            var migrationAssembly = typeof(FocusAppContext).Assembly.GetName().Name;
            builder.Services.AddDbContext<IFocusAppContext, FocusAppContext>(opts =>
            {
                opts.UseSqlite($"Filename={Consts.DatabasePath}", x => x.MigrationsAssembly(migrationAssembly));
            });

            builder.Services
                .RegisterRefitClient()
                .RegisterServices()
                .RegisterPages();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
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
            services.AddSingleton<ITimerService, TimerService>();

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
    }
}