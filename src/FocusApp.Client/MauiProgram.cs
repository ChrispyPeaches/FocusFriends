using System.Reflection;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Clients;
using FocusApp.Shared.Data;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Views;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;
using SimpleToolkit.SimpleShell;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using FocusApp.Client.Views.Shop;
using FocusApp.Client.Views.Social;
using Auth0.OidcClient;
using FluentValidation;
using FocusApp.Client.Configuration.PipelineBehaviors;
using FocusApp.Client.Methods.Sync;

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
                .RegisterPopups()
                .RegisterMediatR();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton(new Auth0Client(new()
            {
                Domain = "dev-7c8vyxbx5myhzmji.us.auth0.com",
                ClientId = "PR3eHq0ehapDGtpYyLl5XFhd1mOQX9uD",
                RedirectUri = "myapp://callback",
                PostLogoutRedirectUri = "myapp://callback",
                Scope = "openid profile email"
            }));

            Task.Run(() => StartupSync(builder.Services));

            PreferenceRequest();

            return builder.Build();
        }

        /// <summary>Registers all the handlers, validators, and behaviors.</summary>
        public static IServiceCollection RegisterMediatR(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
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

            // Singleton User Data
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

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

        private static async Task StartupSync(IServiceCollection services)
        {
            try
            {
                var serviceProvider = services
                    .BuildServiceProvider()
                    .CreateScope()
                    .ServiceProvider;
                FocusAppContext context = serviceProvider.GetRequiredService<FocusAppContext>();
                IAPIClient client = serviceProvider.GetRequiredService<IAPIClient>();

                var syncMindfulnessTips = new SyncMindfulnessTips.Handler(context, client);

                await syncMindfulnessTips.Handle(new SyncMindfulnessTips.Query(), new CancellationToken());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred when syncing mindfulness tips.");
                Console.Write(ex);
            }
        }

        private static void PreferenceRequest()
        {
            var keys = new List<string> { "ambiance_volume", "notifications_enabled", "startup_tips_enabled", "session_rating_enabled" };
            var vals = new List<dynamic> { 50.00, false, true, true };

            var pairs = keys.Zip(vals);

            foreach (var pair in pairs)
            {
                if (!Preferences.Default.ContainsKey(pair.First))
                {
                    Preferences.Default.Set(pair.First, pair.Second);
                }
            }
        }
    }
}