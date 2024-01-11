using FocusApp.Resources;

namespace FocusApp
{
    class App : Application
    {
        /// <summary>
        /// Create a new instance of the application with the styling resources and 
        /// a <see cref="Shell"/> that only acts as a container for the <see cref="MainPage"/>.
        /// </summary>
        public App()
        {
            Resources = new AppStyles();

            MainPage = new ContentPage() { Content = new Views.SettingsView() };
        }
    }
}
