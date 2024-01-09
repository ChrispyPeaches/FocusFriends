using FocusApp.Resources;

namespace FocusApp
{
    class App : Application
    {
        public App()
        {
            Resources = new AppStyles();

            MainPage = new Shell() { CurrentItem = new MainPage() };
        }
    }
}
