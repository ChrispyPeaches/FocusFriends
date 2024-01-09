using FocusApp.Helpers;

namespace FocusApp
{
    public class MainPage : ContentPage
    {
        public MainPage()
        {
            Build();
#if DEBUG
            HotReloadService.UpdateApplicationEvent += ReloadUI;
#endif
        }

        /// <summary>
        /// For the main page, we're putting the UI in a separate method so that we can call it again when Hot Reload is triggered.
        /// </summary>
        public void Build()
        {
            Content = new StackLayout()
            {
                Children =
                {
                    new Label()
                    {
                        Text = "Hello World!"
                    }
                }
            };
        }

        /// <summary>
        /// This method is called when Hot Reload is triggered.
        /// </summary>
        /// <param name="obj"></param>
        private void ReloadUI(Type[] obj)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Build();
            });
        }
    }
}
