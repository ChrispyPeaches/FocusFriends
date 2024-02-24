using System.Diagnostics;
using System.Xml;
using CommunityToolkit.Maui.Converters;
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
        Helpers.PopupService _popupService;
        CarouselView _petsCarouselView { get; set; }
        CarouselView _soundsCarouselView { get; set; }
        CarouselView _furnitureCarouselView { get; set; }
        ActivityIndicator _awaitAPIIndicator { get; set; }
        Popup _awaitAPIPopup { get; set; }

        #region Frontend
        public ShopPage(IAPIClient client, Helpers.PopupService popupService)
        {
            _client = client;
            _popupService = popupService;

            _petsCarouselView = BuildBaseCarouselView();
            _soundsCarouselView = BuildBaseCarouselView();
            _furnitureCarouselView = BuildBaseCarouselView();

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
                    new Grid
                    { 
                        Children =
                        { 
                            new Label
                            { 
                                Text = "20",
                                FontSize = 20,
                                HorizontalOptions = LayoutOptions.Start,
                                VerticalOptions = LayoutOptions.Center,
                            }
                            .Margins(left: 10, right: 10),
                            new Image
                            { 
                                Source = new FileImageSource
                                { 
                                    File = "logo.png"
                                },
                                HeightRequest = 25,
                                WidthRequest = 25,
                                HorizontalOptions = LayoutOptions.Start,
                                VerticalOptions = LayoutOptions.Center,
                            }
                            .Margins(left:40, right:40),
                            // Header
                            new Label
                            {
                                Text = "Shop",
                                FontSize = 30,
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                FontAttributes = FontAttributes.Bold
                            }
                        }
                    },
                    // Horizontal Divider
                    new BoxView
                    {
                        Color = Color.Parse("Black"),
                        WidthRequest = 400,
                        HeightRequest = 2,
                        Margin = 20
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
                        Text = "Furniture",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                    },
                    _furnitureCarouselView
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
                itemImage.SetBinding(
                    ImageButton.SourceProperty, "ImageSource",
                    converter: new ByteArrayToImageSourceConverter());

                itemImage.Clicked += (s,e) =>
                {
                    OnImageButtonClicked(s, e);
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

        void OnImageButtonClicked(object sender, EventArgs eventArgs)
        {
            var itemButton = sender as ImageButton;
            var shopItem = (ShopItem)itemButton.BindingContext;

            var itemPopup = (ShopItemPopupInterface)_popupService.ShowAndGetPopup<ShopItemPopupInterface>();
            itemPopup.PopulatePopup(shopItem);
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
                ImageSource = p.Image,
                Type = ShopItemType.Pets
            })
            .OrderBy(p => p.Price)
            .ToArray();

            _petsCarouselView.ItemsSource = pets;
            _soundsCarouselView.ItemsSource = pets;
            _furnitureCarouselView.ItemsSource = pets;

            //_awaitAPIIndicator.IsRunning = false;
            //_awaitAPIPopup.CloseAsync();

            base.OnAppearing();
        }

        #endregion
    }
}

// Basic class for grouping ShopItem data
public class ShopItem
{ 
    public string Name { get; set; }
    public byte[] ImageSource { get; set; }
    public ShopItemType Type { get; set; }
    public int Price { get; set; }
}

public enum ShopItemType
{ 
    Pets,
    Sounds,
    Furniture
}