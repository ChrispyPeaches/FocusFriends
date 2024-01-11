namespace FocusApp.Views.Social
{
    internal class MainView : ContentView
    {
        public MainView()
        {
            Content = new Grid
            {
                BackgroundColor = Colors.LightGreen, Opacity = 0.9,
                Children =
                {
                    new Label
                    {
                        Text = "Social",
                        FontSize = 30,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                }
            };
        }
    }
}