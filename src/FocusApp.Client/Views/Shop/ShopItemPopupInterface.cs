using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using FocusCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Shapes;

namespace FocusApp.Client.Views.Shop
{
    internal class ShopItemPopupInterface : BasePopup
    {
        Helpers.PopupService _popupService;
        StackLayout _popupContentStack;
        IAuthenticationService _authenticationService;
        IFocusAppContext _localContext;
        IAPIClient _client;
        ILogger<ShopItemPopupInterface> _logger;
        ShopItem _currentItem { get; set; }
        public ShopPage ShopPage { get; set; }

        public ShopItemPopupInterface(Helpers.PopupService popupService, IAuthenticationService authenticationService, IFocusAppContext localContext, IAPIClient client, ILogger<ShopItemPopupInterface> logger)
        {
            _popupService = popupService;
            _authenticationService = authenticationService;
            _localContext = localContext;
            _client = client;
            _logger = logger;

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
            _authenticationService.CurrentUser.Balance -= _currentItem.Price;

            switch (_currentItem.Type)
            {
                case ShopItemType.Pets:
                    try
                    {
                        // Add the user's new pet to the local database
                        User user = await _localContext.Users.FirstAsync(u => u.Id == _authenticationService.CurrentUser.Id);
                        user.Pets?.Add(new UserPet
                        {
                            Pet = await _localContext.Pets.FirstAsync(p => p.Id == _currentItem.Id)
                        });

                        // Update the user's balance on the local database
                        user.Balance = _authenticationService.CurrentUser.Balance;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error adding UserPet to local database.");
                    }

                    // Add the user's pet to the server database
                    // Note: This endpoint additionally updates the user's balance on the server
                    try
                    {
                        await _client.AddUserPet(new AddUserPetCommand
                        {
                            UserId = _authenticationService.CurrentUser.Id,
                            PetId = _currentItem.Id,
                            UpdatedBalance = _authenticationService.CurrentUser.Balance,
                        });
                    }
                    catch (Exception ex) 
                    {
                        _logger.LogError(ex, "Error adding UserPet to server database.");
                    }

                    break;

                case ShopItemType.Decor:
                    // Add the user's new decor to the local database
                    try
                    {
                        User user = await _localContext.Users.FirstAsync(u => u.Id == _authenticationService.CurrentUser.Id);
                        user.Decor?.Add(new UserDecor
                        {
                            Decor = await _localContext.Decor.FirstAsync(f => f.Id == _currentItem.Id)
                        });

                        // Update the user's balance on the local database
                        user.Balance = _authenticationService.CurrentUser.Balance;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error adding UserDecor to local database.");
                    }

                    // Add the user's decor to the server database
                    // Note: This endpoint additionally updates the user's balance on the server
                    try
                    {
                        await _client.AddUserDecor(new AddUserDecorCommand
                        {
                            UserId = _authenticationService.CurrentUser.Id,
                            DecorId = _currentItem.Id,
                            UpdatedBalance = _authenticationService.CurrentUser.Balance,
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error adding UserDecor to server database.");
                    }

                    break;
                case ShopItemType.Islands:
                    try
                    {
                        // Add the user's new island to the local database
                        User user = await _localContext.Users.FirstAsync(u => u.Id == _authenticationService.CurrentUser.Id);
                        user.Islands?.Add(new UserIsland
                        {
                            Island = await _localContext.Islands.FirstAsync(f => f.Id == _currentItem.Id)
                        });

                        // Update the user's balance on the local database
                        user.Balance = _authenticationService.CurrentUser.Balance;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error adding UserIsland to local database.");
                    }

                    // Add the user's island to the server database
                    try
                    {
                        await _client.AddUserIsland(new AddUserIslandCommand
                        {
                            UserId = _authenticationService.CurrentUser.Id,
                            IslandId = _currentItem.Id,
                            UpdatedBalance = _authenticationService.CurrentUser.Balance,
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error adding UserIsland to server database.");
                    }

                    break;

                default:
                    break;
            }

            try
            {
                await _localContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes to local database.");
            }

            // After purchasing item, update the user balance display on the shop page
            ShopPage._balanceLabel.Text = _authenticationService.CurrentUser.Balance.ToString();

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
                    return await _localContext.UserDecor.AnyAsync(f => 
                           f.DecorId == _currentItem.Id
                        && f.UserId == _authenticationService.CurrentUser.Id);
                
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