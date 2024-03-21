using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Views.Social;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using Microsoft.Maui.Controls.Shapes;
using SimpleToolkit.SimpleShell.Extensions;

namespace FocusApp.Client.Views.Shop
{
    internal class EnsureLoginPopupInterface : BasePopup
    {
        PopupService _popupService;
        public string OriginPage;

        public EnsureLoginPopupInterface(PopupService popupService)
        {
            _popupService = popupService;

            // Set popup location
            HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            Color = Colors.Transparent;

            CanBeDismissedByTappingOutsideOfPopup = false;



            Content = new Border
            {
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20, 20, 20, 20) },
                BackgroundColor = AppStyles.Palette.OrchidPink,
                WidthRequest = 360,
                HeightRequest = 360,
                Content = new StackLayout
                {
                    new Label
                    {
                        FontSize = 18.5,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Text = "Log in / sign up to use this feature!"
                    }
                    .Margins(top: 10),
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
                    new Grid
                    {
                        Children =
                        {
                            new Button
                            {
                                WidthRequest = 125,
                                HeightRequest = 50,
                                Text = "No Thanks!",
                                HorizontalOptions = LayoutOptions.Start,
                            }
                            .Margins(left: 35, top: 100)
                            .Invoke(button => button.Pressed += (s,e) => RedirectToTimerPage(s,e)),
                            new Button
                            {
                                WidthRequest = 125,
                                HeightRequest = 50,
                                Text = "Log in/Sign up!",
                                HorizontalOptions = LayoutOptions.End,
                            }
                            .Margins(right: 35, top: 100)
                            .Invoke(button => button.Pressed += (s,e) => RedirectToLoginPage(s,e))
                        }
                    }
                }
            };
        }

        // User doesn't want to login, redirect to timer page
        private async void RedirectToTimerPage(object sender, EventArgs e)
        {
            var appShell = (AppShell)Shell.Current;
            var tabButtons = appShell.tabButtons;

            if (OriginPage == nameof(ShopPage))
            {
                tabButtons[0].BackgroundColor = Colors.Transparent;
                tabButtons[1].BackgroundColor = AppStyles.Palette.LightMauve;
                Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
            }
            else if (OriginPage == nameof(SocialPage))
            {
                tabButtons[2].BackgroundColor = Colors.Transparent;
                tabButtons[1].BackgroundColor = AppStyles.Palette.LightMauve;
                Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
            }

            await Shell.Current.GoToAsync($"///{nameof(TimerPage)}", true);
            _popupService.HidePopup();
        }

        // User wants to login, redirect to login page
        private async void RedirectToLoginPage(object sender, EventArgs e)
        {
            var appShell = (AppShell)Shell.Current;
            var tabButtons = appShell.tabButtons;

            if (OriginPage == nameof(ShopPage))
            {
                tabButtons[0].BackgroundColor = Colors.Transparent;
                tabButtons[1].BackgroundColor = AppStyles.Palette.LightMauve;
                Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
            }
            else if (OriginPage == nameof(SocialPage))
            {
                tabButtons[2].BackgroundColor = Colors.Transparent;
                tabButtons[1].BackgroundColor = AppStyles.Palette.LightMauve;
                Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
            }

            await Shell.Current.GoToAsync($"///{nameof(LoginPage)}", true);
            _popupService.HidePopup();
        }
    }
}