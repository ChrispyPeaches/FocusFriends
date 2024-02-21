using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using CommunityToolkit.Maui.Views;
using FocusApp.Client.Resources;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics.Text;
using SimpleToolkit.SimpleShell.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusApp.Client.Views.Social
{
    public class ProfilePopupInterface : Popup
    {
        private Helpers.PopupService _popupService;

        public ProfilePopupInterface(Helpers.PopupService popupService)
        {
            _popupService = popupService;

            // Set popup location
            HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.End;
            VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Start;
            Color = Colors.Transparent;

            Content = new Border
            {
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20,20,20,20)},
                BackgroundColor = AppStyles.Palette.LightMauve,
                WidthRequest = 200,
                HeightRequest = 280,
                Content = new VerticalStackLayout
                {
                    WidthRequest = 200,
                    HeightRequest = 280,
                    BackgroundColor = AppStyles.Palette.DarkMauve,
                    Children =
                    {
                        new Label()
                        {
                            /*Shadow = new Shadow
                            {
                                Brush = Brush.Black,
                                Offset = new Point(20,20),
                                Radius = 40,
                                Opacity = 0.8f
                            },*/
                            WidthRequest = 210,
                            HeightRequest = 55,
                            BackgroundColor = AppStyles.Palette.LightMauve,
                            FontSize = 30,
                            FontFamily = "OpenSansSemibold",
                            TextColor = Colors.White,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,

                            // Add logic to fetch username
                            Text = "Username"
                        },

                        new Button()
                        {
                            WidthRequest = 210,
                            HeightRequest = 55,
                            BorderWidth = 0.5,
                            BorderColor = AppStyles.Palette.DarkMauve.AddLuminosity(-.05f),
                            BackgroundColor = AppStyles.Palette.DarkMauve,
                            Padding = 0,
                            FontSize = 30,
                            TextColor = Colors.White,
                            Text = "My Profile"
                            //BindingContext = nameof(ProfilePage)
                        },

                        new Button()
                        {
                            WidthRequest = 210,
                            HeightRequest = 55,
                            BorderWidth = 0.5,
                            BorderColor = AppStyles.Palette.DarkMauve.AddLuminosity(-.05f),
                            BackgroundColor = AppStyles.Palette.DarkMauve,
                            Padding = 0,
                            FontSize = 30,
                            TextColor = Colors.White,
                            Text = "My Pets",
                            BindingContext = nameof(PetsPage)
                        }
                        .Invoke(button => button.Released += (sender, eventArgs) =>
                                PageButtonClicked(sender, eventArgs)),

                        new Button()
                        {

                            WidthRequest = 210,
                            HeightRequest = 55,
                            BorderWidth = 0.5,
                            BorderColor = AppStyles.Palette.DarkMauve.AddLuminosity(-.05f),
                            BackgroundColor = AppStyles.Palette.DarkMauve,
                            Padding = 0,
                            FontSize = 30,
                            TextColor = Colors.White,
                            Text = "My Badges"
                            //BindingContext = nameof(BadgesPage)
                        },

                        new Button()
                        {

                            WidthRequest = 210,
                            HeightRequest = 55,
                            BorderWidth = 0.5,
                            BorderColor = AppStyles.Palette.DarkMauve.AddLuminosity(-.05f),
                            BackgroundColor = AppStyles.Palette.DarkMauve,
                            Padding = 0,
                            FontSize = 30,
                            TextColor = Colors.White,
                            Text = "My Settings",
                            BindingContext = nameof(SettingsPage)
                        }
                        .Invoke(button => button.Released += (sender, eventArgs) =>
                                PageButtonClicked(sender, eventArgs)),
                    }
                }
                .Top()
                .Right()
            };
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
            await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{pageName}");
            _popupService.HidePopup();
        }
    }
}
