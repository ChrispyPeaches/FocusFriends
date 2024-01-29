using FocusApp.Clients;
using Refit;
using Microsoft.Maui.Controls;
namespace FocusApp.Views.Shop
{
    internal class MainView : ContentView
    {
        private IAPIClient _client { get; set; }

        public MainView()
        {
            _client = RestService.For<IAPIClient>("http://10.0.2.2:5223");
            //new Action(async () => await LoadData())();
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

        public async Task LoadData()
        {
            var users = await _client.GetUser();
            var g = 8;
        }
    }
}
