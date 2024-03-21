using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using CommunityToolkit.Maui.Views;
using FocusApp.Client.Resources;
using Microsoft.Maui.Controls;
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
    internal class AddFriendPopupInterface : BasePopup
    {
        private Helpers.PopupService _popupService;

        public AddFriendPopupInterface(Helpers.PopupService popupService)
        {
            _popupService = popupService;

            Color = Colors.Transparent;

            List<ImageCell> pendingFriends = new List<ImageCell>();
            for (int i = 0; i < 5; i++)
            {
                pendingFriends.Add(new ImageCell
                {
                    Text = "Friend " + i,
                    ImageSource = new FileImageSource
                    {
                        // Add logic that gets profile picture from pending friend user data
                        File = "dotnet_bot.png"
                    },
                    BindingContext = this
                });
            };

            DataTemplate pendingFriendDataTemplate = new DataTemplate(typeof(ImageCell));
            pendingFriendDataTemplate.SetBinding(ImageCell.TextProperty, "Text");
            pendingFriendDataTemplate.SetBinding(ImageCell.ImageSourceProperty, "ImageSource");

            Content = new Border
            {
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20, 20, 20, 20) },
                BackgroundColor = AppStyles.Palette.LightMauve,
                WidthRequest = 350,
                HeightRequest = 450,
                Content = new VerticalStackLayout
                {
                    WidthRequest = 350,
                    HeightRequest = 450,
                    BackgroundColor = Colors.White,
                    Children =
                    {
                        new Frame()
                        {
                            WidthRequest = 360,
                            HeightRequest = 55,
                            BackgroundColor = AppStyles.Palette.DarkMauve,
                            Content = new Label()
                            {
                                Shadow = new Shadow
                                {
                                    Brush = Brush.Black,
                                    Radius = 5,
                                    Opacity = 0.6f
                                },
                                WidthRequest = 210,
                                HeightRequest = 55,
                                FontSize = 30,
                                TextColor = Colors.White,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,

                                Text = "Add Friend"
                            }
                        },

                        new Frame()
                        {
                            WidthRequest = 360,
                            HeightRequest = 100,
                            BackgroundColor = Colors.Transparent,
                            Content = new Entry
                            {
                                Placeholder = "Enter friend's username",
                                FontSize = 20,
                                TextColor = Colors.Black,
                                ClearButtonVisibility = ClearButtonVisibility.Never,
                                Keyboard = Keyboard.Plain
                            }.Fill()
                        },

                        // Horizontal Divider
                        new BoxView
                        {
                            Color = AppStyles.Palette.DarkMauve,
                            WidthRequest = 360,
                            HeightRequest = 2
                        }
                        .Top(),

                        new Frame()
                        {
                            WidthRequest = 360,
                            HeightRequest = 350,
                            BackgroundColor = Colors.Transparent,
                            Content = new ListView
                            {
                                Header = "Pending Friend Requests",
                                ItemsSource = pendingFriends,
                                ItemTemplate = pendingFriendDataTemplate
                            }
                        },
                    }
                }
                .Top()
                .Right()
            };
        }
    }
}
