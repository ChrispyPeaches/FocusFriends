using System.Reflection;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Clients;
using FocusApp.Shared.Data;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Views;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Refit;
using SimpleToolkit.SimpleShell;
using Auth0.OidcClient;
using FluentValidation;
using FocusApp.Client.Configuration.PipelineBehaviors;
using FocusApp.Client.Methods.Sync;
using static FocusApp.Client.Helpers.PreferencesHelper;

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
            
            Task.Run(() => RunStartupLogic(builder.Services));


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
            services.AddScoped<ISyncService, SyncService>();

            // Registered as scoped so multiple instances can be made for parallel DB operations
            services.AddScoped<IBadgeService, BadgeService>();

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

        /// <summary>
        /// Ensure the database is created and migrations are applied, then run the startup logic.
        /// </summary>
        private static async Task RunStartupLogic(IServiceCollection services)
        {
            try
            {
                var scopedServiceProvider = services
                    .BuildServiceProvider()
                    .CreateScope()
                    .ServiceProvider;
                _ = scopedServiceProvider.GetRequiredService<FocusAppContext>();

                await Task.Run(() => StartupSync(services, default), default);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error running startup logic");
                Console.Write(ex);
            }

        }

        /// <summary>
        /// Syncs all item types in the <see cref="SyncItems.SyncItemType"/> enum between the API and mobile database
        /// if the last sync happened over a week ago or this is the first time starting the app.
        /// </summary>
        /// <remarks>
        /// If there's an unexpected error, the critical data for app functionality will be retrieved.
        /// </remarks>
        private static async Task StartupSync(IServiceCollection services, CancellationToken cancellationToken)
        {
            try
            {

                // If not in debug and a sync has been done in the past week, don't sync
#if !DEBUG
                string lastSyncTimeString = PreferencesHelper.Get<string>(PreferenceNames.last_sync_time);
                if (!string.IsNullOrEmpty(lastSyncTimeString))
                {
                    DateTimeOffset lastSyncTime = DateTimeOffset.Parse(lastSyncTimeString);

                    if (DateTimeOffset.UtcNow < lastSyncTime.AddDays(7))
                        return;
                }
#endif

                IList<Task> tasks = new List<Task>();
                foreach (SyncItems.SyncItemType syncType in Enum.GetValues(typeof(SyncItems.SyncItemType)))
                {
                    IMediator mediator = services
                        .BuildServiceProvider()
                        .CreateScope()
                        .ServiceProvider
                        .GetRequiredService<IMediator>();

                    tasks.Add(
                            Task.Run(() => mediator.Send(
                                        new SyncItems.Query() { ItemType = syncType },
                                        cancellationToken),
                                    cancellationToken)
                        );
                }

                await Task.WhenAll(tasks);

                PreferencesHelper.Set(PreferenceNames.last_sync_time, DateTimeOffset.UtcNow.ToString("O"));
            }
            catch (Exception ex)
            {
                // If there's an error, ensure the essential database information is gathered
                Console.WriteLine("Error occurred when syncing selectable items and mindfulness tips. Running essential database population.");
                Console.Write(ex);
                await GatherEssentialDatabaseData(services);
            }
        }

        /// <summary>
        /// Populates the database with initial data requested from the API for any of
        /// the island, pets, or decor tables if they don't have any entries.
        /// </summary>
        private static async Task GatherEssentialDatabaseData(IServiceCollection services)
        {
            try
            {
                var scopedServiceProvider = services
                    .BuildServiceProvider()
                    .CreateScope()
                    .ServiceProvider;
                IMediator mediator = scopedServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(new SyncInitialData.Query());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when running essential database population.");
                Console.Write(ex);
            }
        }
    }
}