using System.Diagnostics;
using System.Xml;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using FocusApp.Client.Clients;
using FocusApp.Shared.Models;
using FocusCore.Queries.Shop;
using FocusCore.Queries.User;
using Microsoft.Maui.Controls;

namespace FocusApp.Client.Views.Shop
{
    internal class ShopPage : BasePage
    {
        IAPIClient _client;
        CarouselView _petsCarouselView { get; set; }
        CarouselView _soundsCarouselView { get; set; }
        CarouselView _thirdCarouselView { get; set; }
        ActivityIndicator _awaitAPIIndicator { get; set; }
        Popup _awaitAPIPopup { get; set; }

        #region Frontend
        public ShopPage(IAPIClient client)
        {
            _client = client;

            _petsCarouselView = BuildBaseCarouselView();
            _soundsCarouselView = BuildBaseCarouselView();
            _thirdCarouselView = BuildBaseCarouselView();

            /*
            _awaitAPIIndicator = new ActivityIndicator
            {
                IsRunning = true,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromRgba("#FFFFFF00"),
            };

            _awaitAPIPopup = new Popup
            {
                Content = new VerticalStackLayout
                { 
                    Children =
                    {
                        _awaitAPIIndicator
                    }
                }
            };
            */

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
                    // Pets Carousel
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
                    new Label
                    { 
                        Text = "Third Carousel",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                    },
                    _thirdCarouselView
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
                ImageButton itemImage = new ImageButton
                {
                    WidthRequest = 100,
                    HeightRequest = 100,
                };
                itemImage.SetBinding(ImageButton.SourceProperty, "ImageSource");
                itemImage.Clicked += (s,e) =>
                {
                    OnImageButtonClicked();
                };

                // Shop item price
                Label itemPrice = new Label
                {
                    FontSize = 15,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                itemPrice.SetBinding(Label.TextProperty, "Price");

                // Shop item stack for grouping shop item elements
                VerticalStackLayout itemStack = new VerticalStackLayout
                {
                    BackgroundColor = Color.FromRgba("#E0E4FF00"),
                    Padding = 10
                };

                itemStack.Add(itemName);
                itemStack.Add(itemImage);
                itemStack.Add(itemPrice);

                return itemStack;
            });

            // Add space between the carousels - allows room for carousel label
            carouselView.Margins(0, 5, 0, 10);

            return carouselView;
        }

        void OnImageButtonClicked()
        {
            var g = 1;
        }

        #endregion

        #region Backend
        protected override async void OnAppearing()
        {
            //this.ShowPopup(_awaitAPIPopup);

            List<Pet> shopItems = await _client.GetAllShopItems(new GetAllShopItemsQuery());

            ShopItem[] pets = shopItems.Select(p => new ShopItem
            { 
                Name = p.Name,
                Price = p.Price,
                ImageSource = new FileImageSource
                {
                    File = "pet_beans.png"
                },
                Type = ShopItemType.Pets
            }).ToArray();

            _petsCarouselView.ItemsSource = pets;
            _soundsCarouselView.ItemsSource = pets;
            _thirdCarouselView.ItemsSource = pets;

            //_awaitAPIIndicator.IsRunning = false;
            //_awaitAPIPopup.CloseAsync();

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
    public int Price { get; set; }
}

public enum ShopItemType
{ 
    Pets,
    Sounds,
    Misc
}