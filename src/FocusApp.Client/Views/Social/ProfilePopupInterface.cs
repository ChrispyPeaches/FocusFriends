using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Views.Settings;
using Microsoft.Maui.Controls.Shapes;
using SimpleToolkit.SimpleShell.Extensions;

namespace FocusApp.Client.Views.Social
{
    internal class ProfilePopupInterface : BasePopup
    {
        Helpers.PopupService _popupService;
        IAuthenticationService _authenticationService;

        public ProfilePopupInterface(IAuthenticationService authenticationService, Helpers.PopupService popupService)
        {
            _authenticationService = authenticationService;
            _popupService = popupService;

            // Fetch current user's username
            string username = _authenticationService.UserName;

            // Set popup location
            HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.End;
            VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Start;
            Color = Colors.Transparent;

            Closed += ProfilePopupInterface_Closed;

            var borderWidth = 250;
            var rowWidth = 260;

            Content = new Border
            {
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20, 20, 20, 20) },
                BackgroundColor = AppStyles.Palette.LightMauve,
                WidthRequest = borderWidth,
                Content = new VerticalStackLayout
                {
                    WidthRequest = borderWidth,
                    BackgroundColor = AppStyles.Palette.DarkMauve,
                    Children =
                    {
                        // Top of popup (Username)
                        new Frame()
                        {
                            WidthRequest = rowWidth,
                            HeightRequest = 55,
                            BackgroundColor = AppStyles.Palette.LightMauve,
                            Content = new Label()
                            {
                                Shadow = new Shadow
                                {
                                    Brush = Brush.Black,
                                    Radius = 5,
                                    Opacity = 0.6f
                                },
                                WidthRequest= borderWidth,
                                HeightRequest = 30,
                                FontSize = 20,
                                TextColor = Colors.White,

                                // Fetch username
                                Text = username
                            }
                            .TextCenterHorizontal()
                            .TextCenterVertical()
                        },

                        new Frame()
                        {
                            WidthRequest = rowWidth,
                            HeightRequest = 55,
                            BackgroundColor = AppStyles.Palette.DarkMauve,
                            Content = new Button()
                            {
                                Shadow = new Shadow
                                {
                                    Brush = Brush.Black,
                                    Radius = 5,
                                    Opacity = 0.5f
                                },
                                WidthRequest = rowWidth,
                                HeightRequest = 55,
                                BorderWidth = 0.5,
                                BorderColor = AppStyles.Palette.DarkMauve.AddLuminosity(-.05f),
                                BackgroundColor = Colors.Transparent,
                                Padding = 0,
                                FontSize = 30,
                                TextColor = Colors.White,
                                Text = "My Profile",
                                BindingContext = nameof(ProfilePage)
                            }
                            .Invoke(button => button.Released += (sender, eventArgs) =>
                                    PageButtonClicked(sender, eventArgs))
                        },

                        new Frame()
                        {
                            WidthRequest = rowWidth,
                            HeightRequest = 55,
                            BackgroundColor = AppStyles.Palette.DarkMauve,
                            Content = new Button()
                            {
                                Shadow = new Shadow
                                {
                                    Brush = Brush.Black,
                                    Radius = 5,
                                    Opacity = 0.5f
                                },
                                WidthRequest = rowWidth,
                                HeightRequest = 55,
                                BorderWidth = 0.5,
                                BorderColor = AppStyles.Palette.DarkMauve.AddLuminosity(-.05f),
                                BackgroundColor = Colors.Transparent,
                                Padding = 0,
                                FontSize = 30,
                                TextColor = Colors.White,
                                Text = "My Pets",
                                BindingContext = nameof(PetsPage)
                            }
                            .Invoke(button => button.Released += (sender, eventArgs) =>
                                    PageButtonClicked(sender, eventArgs))
                        },

                        new Frame()
                        {
                            WidthRequest = rowWidth,
                            HeightRequest = 55,
                            BackgroundColor = AppStyles.Palette.DarkMauve,
                            Content = new Button()
                            {
                                Shadow = new Shadow
                                {
                                    Brush = Brush.Black,
                                    Radius = 5,
                                    Opacity = 0.5f
                                },
                                WidthRequest = rowWidth,
                                HeightRequest = 55,
                                BorderWidth = 0.5,
                                BorderColor = AppStyles.Palette.DarkMauve.AddLuminosity(-.05f),
                                BackgroundColor = Colors.Transparent,
                                Padding = 0,
                                FontSize = 30,
                                TextColor = Colors.White,
                                Text = "My Decor",
                                BindingContext = nameof(DecorPage)
                            }
                            .Invoke(button => button.Released += (sender, eventArgs) =>
                                    PageButtonClicked(sender, eventArgs))
                        },

                        new Frame()
                        {
                            WidthRequest = rowWidth,
                            HeightRequest = 55,
                            BackgroundColor = AppStyles.Palette.DarkMauve,
                            Content = new Button()
                            {
                                Shadow = new Shadow
                                {
                                    Brush = Brush.Black,
                                    Radius = 5,
                                    Opacity = 0.5f
                                },
                                WidthRequest = rowWidth,
                                HeightRequest = 55,
                                BorderWidth = 0.5,
                                BorderColor = AppStyles.Palette.DarkMauve.AddLuminosity(-.05f),
                                BackgroundColor = Colors.Transparent,
                                Padding = 0,
                                FontSize = 30,
                                TextColor = Colors.White,
                                Text = "My Islands",
                                BindingContext = nameof(IslandsPage)
                            }
                            .Invoke(button => button.Released += (sender, eventArgs) =>
                                    PageButtonClicked(sender, eventArgs))
                        },

                        new Frame()
                        {
                            WidthRequest = rowWidth,
                            HeightRequest = 55,
                            BackgroundColor = AppStyles.Palette.DarkMauve,
                            Content = new Button()
                            {
                                Shadow = new Shadow
                                {
                                    Brush = Brush.Black,
                                    Radius = 5,
                                    Opacity = 0.5f
                                },
                                WidthRequest = rowWidth,
                                HeightRequest = 55,
                                BorderWidth = 0.5,
                                BorderColor = AppStyles.Palette.DarkMauve.AddLuminosity(-.05f),
                                BackgroundColor = Colors.Transparent,
                                Padding = 0,
                                FontSize = 30,
                                TextColor = Colors.White,
                                Text = "My Badges",
                                BindingContext = nameof(BadgesPage)
                            }
                            .Invoke(button => button.Released += (sender, eventArgs) =>
                                    PageButtonClicked(sender, eventArgs))

                        },

                        new Frame()
                        {
                            WidthRequest = rowWidth,
                            HeightRequest = 55,
                            BackgroundColor = AppStyles.Palette.DarkMauve,
                            Content = new Button()
                            {
                                Shadow = new Shadow
                                {
                                    Brush = Brush.Black,
                                    Radius = 5,
                                    Opacity = 0.5f
                                },
                                WidthRequest = rowWidth,
                                HeightRequest = 55,
                                BorderWidth = 0.5,
                                BorderColor = AppStyles.Palette.DarkMauve.AddLuminosity(-.05f),
                                BackgroundColor = Colors.Transparent,
                                Padding = 0,
                                FontSize = 30,
                                TextColor = Colors.White,
                                Text = "My Settings",
                                BindingContext = nameof(SettingsPage)
                            }
                            .Invoke(button => button.Released += (sender, eventArgs) =>
                                    PageButtonClicked(sender, eventArgs)),
                        },

                        new Frame()
                        {
                            WidthRequest = rowWidth,
                            HeightRequest = 55,
                            BackgroundColor = AppStyles.Palette.DarkMauve,
                            Content = new Button()
                            {
                                Shadow = new Shadow
                                {
                                    Brush = Brush.Black,
                                    Radius = 5,
                                    Opacity = 0.5f
                                },
                                WidthRequest = rowWidth,
                                HeightRequest = 55,
                                BorderWidth = 0.5,
                                BorderColor = AppStyles.Palette.DarkMauve.AddLuminosity(-.05f),
                                BackgroundColor = Colors.Transparent,
                                Padding = 0,
                                FontSize = 30,
                                TextColor = Colors.White,
                                Text = "Leaderboards",
                                BindingContext = nameof(LeaderboardsPage)
                            }
                            .Invoke(button => button.Released += (sender, eventArgs) =>
                                    PageButtonClicked(sender, eventArgs)),
                        }
                    }
                }
                .Top()
                .Right()
            };
        }

        private void ProfilePopupInterface_Closed(object? sender, CommunityToolkit.Maui.Core.PopupClosedEventArgs e)
        {
            _popupService.HidePopup(wasDismissedByTappingOutsideOfPopup: true);
        }

        // Navigate to page according to button
        private async void PageButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;

            // Disable button to prevent double execution
            button.IsEnabled = false;

            var pageName = button.BindingContext as string;

            Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);

            // Navigate to page within social (this allows back navigation to work properly)
            await _popupService.HidePopupAsync<ProfilePopupInterface>();
            await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{pageName}");
        }
    }
}
