using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using Microsoft.Maui.Controls.Shapes;

namespace FocusApp.Client.Views.Shop
{
    internal class ShopItemPopupInterface : BasePopup
    {
        Helpers.PopupService _popupService;
        StackLayout _popupContentStack;
        IAuthenticationService _authenticationService;
        IFocusAppContext _localContext;
        ShopItem _currentItem { get; set; }
        public ShopPage ShopPage { get; set; }

        public ShopItemPopupInterface(Helpers.PopupService popupService, IAuthenticationService authenticationService, IFocusAppContext localContext)
        {
            _popupService = popupService;
            _authenticationService = authenticationService;
            _localContext = localContext;

            // Set popup location
            HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            Color = Colors.Transparent;

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

                    //  Note: This is not ideal - I think on first login, we should cache all this info
                    //        and pull these items from the local database.
                    //        This would mean calling the GetAllShopItems api endpoint once on first login
                    //        or if the database does not have the shop items after login.
                    //        (Story for another day)
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
                    
                    // Add the user's new pet to the local database
                    _localContext.UserPets.Add(new UserPet
                    {
                        UserId = _authenticationService.CurrentUser.Id,
                        Pet = _localContext.Pets.First(p => p.Id == _currentItem.Id)
                    });
                    
                    break;

                case ShopItemType.Furniture:

                    if (!_localContext.Furniture.Any(f => f.Id == _currentItem.Id))
                    {
                        _localContext.Furniture.Add(new Furniture
                        {
                            Id = _currentItem.Id,
                            Name = _currentItem.Name,
                            Price = _currentItem.Price,
                            Image = _currentItem.ImageSource
                        });

                        await _localContext.SaveChangesAsync();
                    }

                    // Add the user's new furniture to the local database
                    _localContext.UserFurniture.Add(new UserFurniture
                    {
                        UserId = _authenticationService.CurrentUser.Id,
                        Furniture = _localContext.Furniture.First(f => f.Id == _currentItem.Id)
                    });

                    break;

                case ShopItemType.Sounds:

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

                    // Add the user's new sound to the local database
                    _localContext.UserSounds.Add(new UserSound
                    {
                        UserId = _authenticationService.CurrentUser.Id,
                        Sound = _localContext.Sounds.First(s => s.Id == _currentItem.Id)
                    });

                    break;
                default:
                    break;
            }

            await _localContext.SaveChangesAsync();

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
                case ShopItemType.Furniture:
                    return _localContext.UserFurniture.Any(f => 
                           f.FurnitureId == _currentItem.Id
                        && f.UserId == _authenticationService.CurrentUser.Id);
                case ShopItemType.Sounds:
                    return _localContext.UserSounds.Any(s => 
                           s.SoundId == _currentItem.Id 
                        && s.UserId == _authenticationService.CurrentUser.Id);
                default:
                    return false;
            }
        }
    }
}