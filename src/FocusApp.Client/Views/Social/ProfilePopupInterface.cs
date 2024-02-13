using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using CommunityToolkit.Maui.Views;
using FocusApp.Client.Resources;
using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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

            Content = new VerticalStackLayout
            {
                BackgroundColor = Colors.Transparent,
                Children =
                {
                    new Frame
                    {
                        CornerRadius = 20,
                        BackgroundColor = AppStyles.Palette.LightMauve,
                        Content = new VerticalStackLayout
                        {
                            Children =
                            {
                                new Label()
                                {
                                    FontSize = 30,
                                    TextColor = Colors.White,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Center,

                                    // Add logic to fetch username
                                    Text = "Username"
                                },

                                new Button()
                                {
                                    BackgroundColor = Colors.Transparent,
                                    Padding = 0,
                                    FontSize = 30,
                                    TextColor = Colors.White,
                                    Text = "My Pets",
                                    BindingContext = nameof(PetsPage)
                                }
                                .Invoke(button => button.Released += (sender, eventArgs) =>
                                        PageButtonClicked(sender, eventArgs)),
                            }
                        }
                        .Top()
                        .Right()
                    }
                }
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
