using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Helpers;
using FocusApp.Client.Methods.Shop;
using FocusApp.Client.Resources;
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

        // Row / Column structure for entire page
        enum PageRow { PageHeader, PetsLabel, PetsCarousel, IslandsLabel, IslandsCarousel, DecorLabel, DecorCarousel, TabBarSpace }
        enum PageColumn { Left, Right }

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
                Text = _authenticationService.Balance.ToString(),
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
            }
            .Row(PageRow.PageHeader)
            .Column(PageColumn.Right)
            .CenterVertical()
            .Right()
            .Margins(right: 50);

            Content = new Grid
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                    (PageRow.PageHeader, GridRowsColumns.Stars(0.4)),
                    (PageRow.PetsLabel, GridRowsColumns.Stars(0.25)),
                    (PageRow.PetsCarousel, GridRowsColumns.Stars(1)),
                    (PageRow.IslandsLabel, GridRowsColumns.Stars(0.25)),
                    (PageRow.IslandsCarousel, GridRowsColumns.Stars(1)),
                    (PageRow.DecorLabel, GridRowsColumns.Stars(0.25)),
                    (PageRow.DecorCarousel, GridRowsColumns.Stars(1)),
                    (PageRow.TabBarSpace, Consts.TabBarHeight)
                    ),
                ColumnDefinitions = GridRowsColumns.Columns.Define(
                    (PageColumn.Left, GridRowsColumns.Stars(1)),
                    (PageColumn.Right, GridRowsColumns.Stars(1))
                    ),
                BackgroundColor = Colors.LightYellow,
                Children = 
                {
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
                    .Row(PageRow.PageHeader)
                    .Column(PageColumn.Right)
                    .CenterVertical()
                    .Right()
                    .Margins(right: 10),


                    new Label
                    {
                        Text = "Shop",
                        FontSize = 30,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        FontAttributes = FontAttributes.Bold
                    }
                    .Row(PageRow.PageHeader)
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                    .Center(),

                    // Horizontal Divider
                    new BoxView
                    {
                        Color = Color.Parse("Black"),
                        HeightRequest = 2,
                    }
                    .Row(PageRow.PageHeader)
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                    .Bottom(),

                    new Label
                    {
                        Text = "Pets",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                    }
                    .Row(PageRow.PetsLabel)
                    .Column(PageColumn.Left)
                    .CenterVertical()
                    .Left()
                    .Margins(left: 5),

                    _petsCarouselView
                    .Row(PageRow.PetsCarousel)
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length),

                    new Label
                    {
                        Text = "Islands",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                    }
                    .Row(PageRow.IslandsLabel)
                    .Column(PageColumn.Left)
                    .CenterVertical()
                    .Left()
                    .Margins(left: 5),

                    _islandsCarouselView
                    .Row(PageRow.IslandsCarousel)
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length),

                    new Label
                    {
                        Text = "Decor",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Start,
                    }
                    .Row(PageRow.DecorLabel)
                    .Column(PageColumn.Left)
                    .CenterVertical()
                    .Left()
                    .Margins(left: 5),

                    _decorCarouselView
                    .Row(PageRow.DecorCarousel)
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length),
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
                    //Padding = 10
                };

                itemStack.Add(itemName);
                itemStack.Add(itemImage);
                itemStack.Add(itemPrice);

                return itemStack;
            });

            // Add space between the carousels - allows room for carousel label
            //carouselView.Margins(0, 5, 0, 10);

            carouselView.BackgroundColor = AppStyles.Palette.MintGreen;

            return carouselView;
        }

        #endregion

        #region Backend

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (string.IsNullOrEmpty(_authenticationService.Auth0Id))
            {
                var loginPopup = (EnsureLoginPopupInterface)_popupService.ShowAndGetPopup<EnsureLoginPopupInterface>();
                loginPopup.OriginPage = nameof(ShopPage);
            }

            // Update user balance upon showing shop page
            _balanceLabel.Text = _authenticationService.Balance.ToString();

            // If the startup sync task is not completed, show the loading popup and wait for it to complete
            if (_authenticationService.StartupSyncTask != null && !_authenticationService.StartupSyncTask.IsCompleted)
            {
                await _popupService.ShowPopupAsync<SyncDataLoadingPopupInterface>();

                _authenticationService.StartupSyncTask.ContinueWith(async (_) =>
                {
                    await Task.Run(PopulateShopCarousels);

                    await _popupService.HidePopupAsync<SyncDataLoadingPopupInterface>();
                });
            }
            else
            {
                Task.Run(PopulateShopCarousels);
            }
        }

        private async Task PopulateShopCarousels()
        {
            List<ShopItem> shopItems = await _mediator.Send(new GetLocalShopItems.Query());

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                _petsCarouselView.ItemsSource = shopItems.Where(p => p.Type == ShopItemType.Pets);
                _islandsCarouselView.ItemsSource = shopItems.Where(p => p.Type == ShopItemType.Islands);
                _decorCarouselView.ItemsSource = shopItems.Where(p => p.Type == ShopItemType.Decor);
            });
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