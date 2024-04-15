using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Commands.User;
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

        public void PopulatePopup(ShopItem item)
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

            if (UserOwnsItem())
            {
                buyButton.IsEnabled = false;
                buyButton.Opacity = 0.5;
                buyButton.Text = "You own me!";
            }
            else if (_authenticationService.CurrentUser.Balance < item.Price)
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

                    // If the local database currently does not have the pet, store it now
                    // Note: This check will be made obsolete after the shop item sync update
                    if (!_localContext.Pets.Any(p => p.Id == _currentItem.Id))
                    {
                        _localContext.Pets.Add(new Pet
                        {
                            Id = _currentItem.Id,
                            Name = _currentItem.Name,
                            Price = _currentItem.Price,
                            Image = _currentItem.ImageSource
                        });

                        await _localContext.SaveChangesAsync();
                    }

                    try
                    {
                        // Add the user's new pet to the local database
                        User user = await _localContext.Users.FirstOrDefaultAsync(u => u.Id == _authenticationService.CurrentUser.Id);
                        user.Pets?.Add(new UserPet
                        {
                            Pet = _localContext.Pets.First(p => p.Id == _currentItem.Id)
                        });

                        // Update the user's balance on the local database
                        user.Balance = _authenticationService.CurrentUser.Balance;
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Error, "Error adding UserPet to local database. Exception: " + ex.Message);
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
                        _logger.Log(LogLevel.Error, "Error adding UserPet to server database. Exception: " + ex.Message);
                    }

                    break;

                case ShopItemType.Decor:

                    // Note: This check will be made obsolete after the shop item sync update
                    if (!_localContext.Decor.Any(f => f.Id == _currentItem.Id))
                    {
                        _localContext.Decor.Add(new Decor
                        {
                            Id = _currentItem.Id,
                            Name = _currentItem.Name,
                            Price = _currentItem.Price,
                            Image = _currentItem.ImageSource
                        });

                        await _localContext.SaveChangesAsync();
                    }

                    // Add the user's new decor to the local database
                    try
                    {
                        User user = await _localContext.Users.FirstOrDefaultAsync(u => u.Id == _authenticationService.CurrentUser.Id);
                        user.Decor?.Add(new UserDecor
                        {
                            Decor = _localContext.Decor.First(f => f.Id == _currentItem.Id)
                        });

                        // Update the user's balance on the local database
                        user.Balance = _authenticationService.CurrentUser.Balance;
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Error, "Error adding UserDecor to local database. Exception: " + ex.Message);
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
                        _logger.Log(LogLevel.Error, "Error adding UserDecor to server database. Exception: " + ex.Message);
                    }

                    break;
                /*
                case ShopItemType.Sounds:

                    // Note: This check will be made obsolete after the shop item sync update
                    if (!_localContext.Sounds.Any(s => s.Id == _currentItem.Id))
                    {
                        _localContext.Sounds.Add(new Sound
                        {
                            Id = _currentItem.Id,
                            Name = _currentItem.Name,
                            Price = _currentItem.Price,
                            Image = _currentItem.ImageSource
                        });

                        await _localContext.SaveChangesAsync();
                    }

                    try
                    {
                        // Add the user's new sound to the local database
                        User user = await _localContext.Users.FirstOrDefaultAsync(u => u.Id == _authenticationService.CurrentUser.Id);
                        user.Sounds?.Add(new UserSound
                        {
                            Sound = _localContext.Sounds.First(f => f.Id == _currentItem.Id)
                        });

                        // Update the user's balance on the local database
                        user.Balance = _authenticationService.CurrentUser.Balance;
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Error, "Error adding UserSound to local database. Exception: " + ex.Message);
                    }

                    // Add the user's sound to the server database
                    // Note: This endpoint additionally updates the user's balance on the server
                    // If time allows, we will store the sound files on the server, and fetch/store them after purchase
                    try
                    {
                        await _client.AddUserIsland(new AddUserIslandCommand
                        {
                            UserId = _authenticationService.CurrentUser.Id,
                            SoundId = _currentItem.Id,
                            UpdatedBalance = _authenticationService.CurrentUser.Balance,
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Error, "Error adding UserSound to server database. Exception: " + ex.Message);
                    }

                    break;
                */
                default:
                    break;
            }

            try
            {
                await _localContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Error saving changes to local database. Exception: " + ex.Message);
            }

            // After purchasing item, update the user balance display on the shop page
            ShopPage._balanceLabel.Text = _authenticationService.CurrentUser.Balance.ToString();

            _popupService.HidePopup();
        }

        private bool UserOwnsItem()
        {
            switch (_currentItem.Type)
            {
                case ShopItemType.Pets:
                    return _localContext.UserPets.Any(p =>
                           p.PetId == _currentItem.Id
                        && p.UserId == _authenticationService.CurrentUser.Id);
                case ShopItemType.Decor:
                    return _localContext.UserDecor.Any(f => 
                           f.DecorId == _currentItem.Id
                        && f.UserId == _authenticationService.CurrentUser.Id);
                /*
                case ShopItemType.Sounds:
                    return _localContext.UserSounds.Any(s => 
                           s.SoundId == _currentItem.Id 
                        && s.UserId == _authenticationService.CurrentUser.Id);
                */
                default:
                    return false;
            }
        }
    }
}