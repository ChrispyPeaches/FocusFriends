using FocusApp.Clients;
using FocusCore.Queries.User;

namespace FocusApp.Views.Shop
{
    internal class MainPage : ContentPage
    {
        IAPIClient _client;
        public MainPage(IAPIClient client)
        {
            _client = client;
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
                    },
                    new CarouselView()
                }
            };
        }

        protected override async void OnAppearing()
        {
            var user = await _client.GetUser(new GetUserQuery { Id = Guid.NewGuid() });
            base.OnAppearing();
        }
    }
}
