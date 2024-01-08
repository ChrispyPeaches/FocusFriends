namespace FocusApp.Views.Social
{
    internal class MainView : ContentView
    {
        public MainView()
        {
            Content = new Grid
            {
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