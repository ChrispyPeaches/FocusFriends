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

        private void ReloadUI(Type[] obj)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Build();
            });
        }
    }
}
