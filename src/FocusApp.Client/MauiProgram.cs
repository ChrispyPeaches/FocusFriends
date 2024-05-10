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
using Microsoft.IdentityModel.Logging;

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
                .RegisterMediatR()
                .RegisterAuthenticationServices();

#if DEBUG
            builder.Logging.AddDebug();
            IdentityModelEventSource.ShowPII = true;
#endif

            return builder.Build();
        }

        public static IServiceCollection RegisterAuthenticationServices(this IServiceCollection services)
        {
#if DEBUG
            const string domain = "dev-7c8vyxbx5myhzmji.us.auth0.com";
            const string clientId = "PR3eHq0ehapDGtpYyLl5XFhd1mOQX9uD";
#else
            const string domain = "zenpxl.us.auth0.com";
            const string clientId = "tQ6cctnvL3AoyXNEEy7YGe5eYtJIewaC";
#endif
            services.AddSingleton(new Auth0Client(new()
            {
                Domain = domain,
                ClientId = clientId,
                RedirectUri = "myapp://callback",
                PostLogoutRedirectUri = "myapp://callback",
                Scope = "openid profile email",
            }));

            services.AddSingleton(new UserManager(domain, clientId));

            return services;
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
#if DEBUG
            string apiDomain = "http://10.0.2.2:5223";
#else
            string apiDomain = "http://prod.zenpxl.com:25565";
#endif

            services
                .AddRefitClient<IAPIClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiDomain));

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
    }
}