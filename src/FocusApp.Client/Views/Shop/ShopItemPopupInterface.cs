using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Helpers;
using FocusApp.Client.Methods.Shop;
using FocusApp.Client.Resources;
using FocusApp.Client.Views.Mindfulness;
using FocusApp.Shared.Data;
using FocusCore.Extensions;
using FocusCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Shapes;
using FocusApp.Client.Extensions;

namespace FocusApp.Client.Views.Shop
{
    internal class ShopItemPopupInterface : BasePopup
    {
        Helpers.PopupService _popupService;
        StackLayout _popupContentStack;
        IAuthenticationService _authenticationService;
        IFocusAppContext _localContext;
        ILogger<ShopItemPopupInterface> _logger;
        IMediator _mediator;
        IBadgeService _badgeService;
        ShopItem _currentItem { get; set; }
        public ShopPage ShopPage { get; set; }

        public ShopItemPopupInterface(PopupService popupService, IAuthenticationService authenticationService, IFocusAppContext localContext, ILogger<ShopItemPopupInterface> logger, IMediator mediator, IBadgeService badgeService)
        {
            _popupService = popupService;
            _authenticationService = authenticationService;
            _localContext = localContext;
            _logger = logger;
            _mediator = mediator;
            _badgeService = badgeService;

            // Set popup location
            HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            Color = Colors.Transparent;

            CanBeDismissedByTappingOutsideOfPopup = false;

            _popupContentStack = new StackLayout();

            Content = new Border
            {
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20,20,20,20)},
                BackgroundColor = AppStyles.Palette.LightMauve,
                WidthRequest = 360,
                HeightRequest = 460,
                Content = _popupContentStack
            };
        }

        public async Task PopulatePopup(ShopItem item)
        {
            _currentItem = item;

            // Shop item name label
            Label itemName = new Label
            {
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
            .Margins(top: 10);
            itemName.SetBinding(Label.TextProperty, "Name");
            itemName.BindingContext = item;

            // Horizontal Divider
            BoxView divider = new BoxView
            {
                Color = Color.Parse("Black"),
                WidthRequest = 400,
                HeightRequest = 2,
                Margin = 20
            }
            .Bottom()
            .Row(0)
            .Column(0)
            .ColumnSpan(2);

            // Shop item image
            Image itemImage = new Image
            {
                WidthRequest = 150,
                HeightRequest = 150
            };
            itemImage.SetBinding(
                Image.SourceProperty, "ImageSource",
                converter: new ByteArrayToImageSourceConverter());
            itemImage.BindingContext = item;

            // Shop item price
            Label itemPrice = new Label
            {
                FontSize = 30,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
            .Margins(top: 10);
            itemPrice.SetBinding(Label.TextProperty, "Price");
            itemPrice.BindingContext = item;

            Button buyButton = new Button
            {
                WidthRequest = 125,
                HeightRequest = 50,
                Text = "Buy Me!",
                HorizontalOptions = LayoutOptions.End,
            }
            .Margins(right: 35, top: 50)
            .Invoke(button => button.Pressed += (s, e) => PurchaseItem(s, e));

            if (await UserOwnsItem())
            {
                buyButton.IsEnabled = false;
                buyButton.Opacity = 0.5;
                buyButton.Text = "You own me!";
            }
            else if (_authenticationService.CurrentUser?.Balance < item.Price)
            {
                buyButton.IsEnabled = false;
                buyButton.Opacity = 0.5;
                buyButton.Text = "Focus more!";
            }
            
            Grid popupButtons = new Grid
            {
                Children =
                {
                    new Button
                    {
                        WidthRequest = 125,
                        HeightRequest = 50,
                        Text = "Leave Me Be",
                        HorizontalOptions = LayoutOptions.Start,
                    }
                    .Margins(left: 35, top: 50)
                    .Invoke(button => button.Pressed += (s,e) => ExitItemPopup(s,e)),
                    buyButton
                }
            };

            _popupContentStack.Add(itemName);
            _popupContentStack.Add(divider);
            _popupContentStack.Add(itemImage);
            _popupContentStack.Add(itemPrice);
            _popupContentStack.Add(popupButtons);
        }

        // User doesn't want to purchase the item, hide popup
        private async void ExitItemPopup(object sender, EventArgs e)
        {
            _popupService.HidePopup();
        }

        // User wants to purchase item, save to local database, reduce balance, hide popup
        private async void PurchaseItem(object sender, EventArgs e)
        {
            try
            {
                await _mediator.Send(new PurchaseItem.Command { Item = _currentItem }, default);

                // Update user's balance within the CurrentUser model
                _authenticationService.CurrentUser.Balance -= _currentItem.Price;

                // After purchasing item, update the user balance display on the shop page
                ShopPage._balanceLabel.Text = _authenticationService.CurrentUser.Balance.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while purchasing item.");
            }

            try
            {
                BadgeEligibilityResult result = await _badgeService.CheckPurchaseBadgeEligibility(_currentItem, default);
                if (result.IsEligible)
                {
                    // Need to implement some sort of generic event that will display a popup within the app with badge info
                    Action badgeEvent = ShopPage.TriggerBadgeEvent;
                    badgeEvent.RunInNewThread();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking for badge eligibility.");
            }

            _popupService.HidePopup();
        }

        private async Task<bool> UserOwnsItem()
        {
            switch (_currentItem.Type)
            {
                case ShopItemType.Pets:
                    return await _localContext.UserPets.AnyAsync(p =>
                           p.PetId == _currentItem.Id
                        && p.UserId == _authenticationService.CurrentUser.Id);
                case ShopItemType.Decor:
                    return await _localContext.UserDecor.AnyAsync(d => 
                           d.DecorId == _currentItem.Id
                        && d.UserId == _authenticationService.CurrentUser.Id);
                
                case ShopItemType.Islands:
                    return await _localContext.UserIslands.AnyAsync(i => 
                           i.IslandId == _currentItem.Id 
                        && i.UserId == _authenticationService.CurrentUser.Id);
                
                default:
                    return false;
            }
        }
    }
}