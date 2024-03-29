using FocusApp.Client.Clients;
using FocusApp.Shared.Data;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Views.Shop;
using SimpleToolkit.SimpleShell;

namespace FocusApp.Client
{
    class App : Application
    {
        public static event Action? WindowCreated;
        public static event Action? WindowActivated;
        public static event Action? WindowDeactivated;
        public static event Action? WindowStopped;
        public static event Action? WindowResumed;
        public static event Action? WindowDestroying;

        /// <summary>
        /// Create a new instance of the application with the styling resources and 
        /// a <see cref="Shell"/> that only acts as a container for the <see cref="MainPage"/>.
        /// </summary>
        public App()
        {
            Resources = new AppStyles();

            MainPage = new AppShell();
        }

        /// <summary>
        /// Setup the app to relay app lifecycle events to static action events.
        /// </summary>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = base.CreateWindow(activationState);

            window.Created += (object? sender, EventArgs e) => WindowCreated?.Invoke();
            window.Activated += (object? sender, EventArgs e) => WindowActivated?.Invoke();
            window.Deactivated += (object? sender, EventArgs e) => WindowDeactivated?.Invoke();
            window.Stopped += (object? sender, EventArgs e) => WindowStopped?.Invoke();
            window.Resumed += (object? sender, EventArgs e) => WindowResumed?.Invoke();
            window.Destroying += (object? sender, EventArgs e) => WindowDestroying?.Invoke();

            return window;
        }
    }
}
