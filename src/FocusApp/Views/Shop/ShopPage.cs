using System.Xml;
using CommunityToolkit.Maui.Markup;
using FocusApp.Clients;
using FocusCore.Queries.User;
using Microsoft.Maui.Controls;

namespace FocusApp.Views.Shop
{
    internal class ShopPage : ContentPage
    {
        IAPIClient _client;
        CarouselView _petsCarouselView { get; set; }
        CarouselView _soundsCarouselView { get; set; }

        #region Frontend
        public ShopPage(IAPIClient client)
        {
            _client = client;

            _petsCarouselView = BuildBaseCarouselView();
            _soundsCarouselView = BuildBaseCarouselView();

            Content = new StackLayout
            {
                BackgroundColor = Colors.LightYellow,
                Children =
                {
                    // Header
                    new Label
                    {
                        Text = "Shop",
                        FontSize = 30,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    },
                    // Horizontal Divider
                    new BoxView
                    {
                        Color = Color.Parse("Black"),
                        WidthRequest = 400,
                        HeightRequest = 2,
                        Margin = 30
                    }
                    .Bottom()
                    .Row(0)
                    .Column(0)
                    .ColumnSpan(2),
                    // Pets Carousel Label
                    new Label
                    {
                        Text = "Pets",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                    },
                    _petsCarouselView,
                    // Sounds Carousel Label
                    new Label
                    {
                        Text = "Sounds",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                    },
                    _soundsCarouselView,
                }
            };
        }

        private CarouselView BuildBaseCarouselView()
        {
            CarouselView carouselView = new CarouselView();

            carouselView.ItemTemplate = new DataTemplate(() =>
            {
                // Shop item name label
                Label itemName = new Label
                {
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                itemName.SetBinding(Label.TextProperty, "Name");

                // Shop item image
                Image itemImage = new Image
                {
                    WidthRequest = 100,
                    HeightRequest = 100
                };
                itemImage.SetBinding(Image.SourceProperty, "ImageSource");

                // Shop item stack for grouping shop item elements
                VerticalStackLayout itemStack = new VerticalStackLayout
                {
                    BackgroundColor = Color.FromRgba("#E0E4FF"),
                    Padding = 10
                };

                itemStack.Add(itemName);
                itemStack.Add(itemImage);

                return itemStack;
            });

            // Add space between the carousels - allows room for carousel label
            carouselView.Margins(0, 5, 0, 10);
            
            // Cant seem to get the indicator view to work - not sure what I'm doing wrong here
            /*
            IndicatorView indicatorView = new IndicatorView
            {
                IndicatorSize = 10,
                Margin = 500,
                HorizontalOptions = LayoutOptions.Center,
            };

            carouselView.IndicatorView = indicatorView;
            */
            return carouselView;
        }

        #endregion

        #region Backend
        protected override async void OnAppearing()
        {
            var user = await _client.GetUser(new GetUserQuery { Id = Guid.NewGuid() });
            // Logic for populating carousels is here so that API calls may be made to fetch data
            // Note: It'd be best to fetch all shop items with one API call, then parse by ShopItemType
            //       to populate the appropriate shop item carousel
            ShopItem[] shopItems = GetPets();
            _petsCarouselView.ItemsSource = shopItems;
            _soundsCarouselView.ItemsSource = shopItems;

            base.OnAppearing();
        }

        // Test function for implementing UI - this will be replaced by API call
        private ShopItem[] GetPets()
        { 
            return
            [
                new ShopItem
                {
                    Name = "Frog",
                    ImageSource = new FileImageSource
                    {
                        File = "pet_beans.png"
                    },
                    Type = ShopItemType.Pets
                },
                new ShopItem
                {
                    Name = "Big Frog",
                    ImageSource = new FileImageSource
                    {
                        File = "pet_bob.png"
                    },
                    Type = ShopItemType.Pets
                },
                new ShopItem
                {
                    Name = "Bigger Frog",
                    ImageSource = new FileImageSource
                    {
                        File = "pet_danole.png"
                    },
                    Type = ShopItemType.Pets
                },
                new ShopItem
                {
                    Name = "Even Bigger Frog",
                    ImageSource = new FileImageSource
                    {
                        File = "pet_franklin.png"
                    },
                    Type = ShopItemType.Pets
                },
                new ShopItem
                {
                    Name = "Humongous Fucker",
                    ImageSource = new FileImageSource
                    {
                        File = "pet_greg.png"
                    },
                    Type = ShopItemType.Pets
                },
            ];
        }
        #endregion
    }
}

// Basic class for grouping ShopItem data
public class ShopItem
{ 
    public string Name { get; set; }
    public FileImageSource ImageSource { get; set; }
    public ShopItemType Type { get; set; }
}

public enum ShopItemType
{ 
    Pets,
    Sounds
}