using System.Diagnostics;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Queries.Shop;

namespace FocusApp.Client.Views.Shop
{
    internal class ShopPage : BasePage
    {
        IAPIClient _client;
        IAuthenticationService _authenticationService;
        FocusAppContext _localContext;
        Helpers.PopupService _popupService;
        CarouselView _petsCarouselView { get; set; }
        CarouselView _soundsCarouselView { get; set; }
        CarouselView _furnitureCarouselView { get; set; }

        public Label _balanceLabel { get; set; }

        #region Frontend

        public ShopPage(IAPIClient client, IAuthenticationService authenticationService, Helpers.PopupService popupService, FocusAppContext localContext)
        {
            _client = client;
            _popupService = popupService;
            _authenticationService = authenticationService;
            _localContext = localContext;

            _petsCarouselView = BuildBaseCarouselView();
            _soundsCarouselView = BuildBaseCarouselView();
            _furnitureCarouselView = BuildBaseCarouselView();


            // Currency text
            _balanceLabel = new Label
            {
                Text = _authenticationService.CurrentUser?.Balance.ToString(),
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
            }
            .Margins(left: 10, right: 10);


            Content = new StackLayout
            {
                BackgroundColor = Colors.LightYellow,
                Children =
                {
                    new Grid
                    { 
                        Children =
                        { 
                            // Currency text
                            _balanceLabel,
                            // Currency icon
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
                            .Margins(left:60, right:40),
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
                    // Furniture Carousel Label
                    new Label
                    { 
                        Text = "Furniture",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                    },
                    // Furniture Carousel
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

            carouselView.BackgroundColor = AppStyles.Palette.MintGreen;

            return carouselView;
        }

        void OnImageButtonClicked(object sender, EventArgs eventArgs)
        {
            var itemButton = sender as ImageButton;
            var shopItem = (ShopItem)itemButton.BindingContext;

            var itemPopup = (ShopItemPopupInterface)_popupService.ShowAndGetPopup<ShopItemPopupInterface>();
            // Give the popup a reference to the shop page so that the displayed user balance can be updated if necessary
            itemPopup.ShopPage = this;
            itemPopup.PopulatePopup(shopItem);
        }

        #endregion

        #region Backend

        protected override async void OnAppearing()
        {
            if (string.IsNullOrEmpty(_authenticationService.AuthToken))
            {
                var loginPopup = (EnsureLoginPopupInterface)_popupService.ShowAndGetPopup<EnsureLoginPopupInterface>();
                loginPopup.OriginPage = nameof(ShopPage);
            }

            // Update user balance upon showing shop page
            _balanceLabel.Text = _authenticationService.CurrentUser?.Balance.ToString();

            // Note: This is temporary - will be made obsolete by shop item sync update
            List<ShopItem> shopItems;
            if (ShopItemsFetched())
            {
                shopItems = GetLocalShopItems();
            }
            else
            {
                shopItems = await _client.GetAllShopItems(new GetAllShopItemsQuery(), default);
            }
            
            // TODO: Replace above logic with fetch from local database
            //List<ShopItem> shopItems = GetAllShopItems();

            shopItems = shopItems.OrderBy(p => p.Price).ToList();

            _petsCarouselView.ItemsSource = shopItems.Where(p => p.Type == ShopItemType.Pets);
            _soundsCarouselView.ItemsSource = shopItems.Where(p => p.Type == ShopItemType.Sounds);
            _furnitureCarouselView.ItemsSource = shopItems.Where(p => p.Type == ShopItemType.Furniture);

            base.OnAppearing();
        }
       

        // Note: This is temporary - will be made obsolete by shop item sync update
        private bool ShopItemsFetched()
        {
            return 
                   _localContext.Pets.Count() == 7
                && _localContext.Furniture.Count() == 6
                /*&& _localContext.Sounds.Count() == 6 */;
        }

        // Gather shop items from local database, and convert to ShopItem objects
        private List<ShopItem> GetLocalShopItems()
        {
            List<ShopItem> pets = _localContext.Pets.Where(p => p.Price > 0).Select(p => new ShopItem
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageSource = p.Image,
                Type = ShopItemType.Pets,
            }).ToList();

            List<ShopItem> furniture = _localContext.Furniture.Select(f => new ShopItem
            {
                Id = f.Id,
                Name = f.Name,
                Price = f.Price,
                ImageSource = f.Image,
                Type = ShopItemType.Furniture
            }).ToList();
            /*
            List<ShopItem> sounds = _localContext.Sounds.Select(s => new ShopItem
            {
                Id = s.Id,
                Name = s.Name,
                Price = s.Price,
                ImageSource = s.Image,
                Type = ShopItemType.Sounds
            }).ToList();
            */
            return pets.Concat(furniture).ToList();
        }

        #endregion
    }
}