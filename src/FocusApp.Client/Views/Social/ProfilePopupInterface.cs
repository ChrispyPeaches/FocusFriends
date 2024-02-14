using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using CommunityToolkit.Maui.Views;
using FocusApp.Client.Resources;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics.Text;
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
                HeightRequest = 250,
                Content = new VerticalStackLayout
                {
                    WidthRequest = 200,
                    HeightRequest = 250,
                    BackgroundColor = AppStyles.Palette.DarkMauve,
                    Children =
                    {
                        new Label()
                        {
                            WidthRequest = 210,
                            HeightRequest = 55,
                            BackgroundColor = AppStyles.Palette.LightMauve,
                            FontSize = 30,
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
                        }
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
            var pageName = button.BindingContext as string;

            await Shell.Current.GoToAsync("///" + pageName);
            _popupService.HidePopup();
        }
    }
}
