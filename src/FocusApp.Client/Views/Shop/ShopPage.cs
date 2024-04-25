using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Helpers;
using FocusApp.Client.Methods.Shop;
using FocusApp.Client.Resources;
using FocusApp.Shared.Data;
using FocusCore.Models;
using MediatR;

namespace FocusApp.Client.Views.Shop
{
    internal class ShopPage : BasePage
    {
        IAuthenticationService _authenticationService;
        PopupService _popupService;
        IMediator _mediator;
        CarouselView _petsCarouselView { get; set; }
        CarouselView _islandsCarouselView { get; set; }
        CarouselView _decorCarouselView { get; set; }

        public Label _balanceLabel { get; set; }

        #region Frontend

        public ShopPage(IAuthenticationService authenticationService, PopupService popupService, IMediator mediator)
        {
            _popupService = popupService;
            _authenticationService = authenticationService;
            _mediator = mediator;

            _petsCarouselView = BuildBaseCarouselView();
            _islandsCarouselView = BuildBaseCarouselView();
            _decorCarouselView = BuildBaseCarouselView();

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
                                    File = "coin.png"
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
                    // Islands Carousel Label
                    new Label
                    {
                        Text = "Islands",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                    },
                    // Islands Carousel
                    _islandsCarouselView,
                    // Decor Carousel Label
                    new Label
                    { 
                        Text = "Decor",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                    },
                    // Decor Carousel
                    _decorCarouselView
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

                itemImage.Clicked += async (s,e) =>
                {
                    await OnImageButtonClicked(s, e);
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

        #endregion

        #region Backend

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (string.IsNullOrEmpty(_authenticationService.AuthToken))
            {
                var loginPopup = (EnsureLoginPopupInterface)_popupService.ShowAndGetPopup<EnsureLoginPopupInterface>();
                loginPopup.OriginPage = nameof(ShopPage);
            }

            // Update user balance upon showing shop page
            _balanceLabel.Text = _authenticationService.CurrentUser?.Balance.ToString();

            List<ShopItem> shopItems = await _mediator.Send(new GetLocalShopItems.Query(), default);

            _petsCarouselView.ItemsSource = shopItems.Where(p => p.Type == ShopItemType.Pets);
            _islandsCarouselView.ItemsSource = shopItems.Where(p => p.Type == ShopItemType.Islands);
            _decorCarouselView.ItemsSource = shopItems.Where(p => p.Type == ShopItemType.Decor);
        }

        async Task OnImageButtonClicked(object sender, EventArgs eventArgs)
        {
            var itemButton = sender as ImageButton;
            var shopItem = (ShopItem)itemButton.BindingContext;

            var itemPopup = (ShopItemPopupInterface)_popupService.ShowAndGetPopup<ShopItemPopupInterface>();
            // Give the popup a reference to the shop page so that the displayed user balance can be updated if necessary
            itemPopup.ShopPage = this;
            await itemPopup.PopulatePopup(shopItem);
        }

        #endregion
    }
}