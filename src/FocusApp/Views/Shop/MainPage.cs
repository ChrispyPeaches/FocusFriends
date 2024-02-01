namespace FocusApp.Views.Shop
{
    internal class MainPage : ContentPage
    {
        public MainPage()
        {
            Content = new Grid
            {
                BackgroundColor = Colors.LightYellow,
                Children =
                {
                    new Label
                    {
                        Text = "Shop",
                        FontSize = 30,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                }
            };
        }
    }
}
