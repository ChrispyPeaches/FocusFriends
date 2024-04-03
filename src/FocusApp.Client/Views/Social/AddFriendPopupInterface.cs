using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using CommunityToolkit.Maui.Views;
using FocusApp.Client.Clients;
using FocusApp.Client.Resources;
using FocusApp.Shared.Models;
using FocusCore.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics.Text;
using SimpleToolkit.SimpleShell.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using FriendRequest = FocusApp.Shared.Models.FriendRequest;

namespace FocusApp.Client.Views.Social
{
    internal class AddFriendPopupInterface : BasePopup
    {
        IAPIClient _client;
        Helpers.PopupService _popupService;
        ListView _friendrequestView { get; set; }

        public AddFriendPopupInterface(IAPIClient client, Helpers.PopupService popupService)
        {
            _client = client;
            _popupService = popupService;

            _friendrequestView = BuildFriendRequestListView();

            Color = Colors.Transparent;

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
                            Content = new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new Entry
                                    {
                                        Placeholder = "Enter friend's username",
                                        FontSize = 20,
                                        TextColor = Colors.Black,
                                        ClearButtonVisibility = ClearButtonVisibility.Never,
                                        Keyboard = Keyboard.Plain
                                    }
                                    .Fill(),

                                    new Button
                                    {
                                        Text = "Send",
                                        WidthRequest = 100,
                                        HeightRequest = 50,
                                        FontSize = 20
                                    }
                                }
                            }
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
                            Content = _friendrequestView
                            
                            /*new ListView
                            {
                                Header = "Pending Friend Requests",
                                ItemsSource = pendingFriends,
                                ItemTemplate = pendingFriendDataTemplate
                            }*/
                        },
                    }
                }
                .Top()
                .Right()
            };

            // Populate PendingFriends upon popup open
            PopulatePopup();
        }

        private ListView BuildFriendRequestListView()
        {
            ListView listView = new ListView();
            listView.Header = "Pending Friend Requests";


            listView.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell cell = new ViewCell();
                HorizontalStackLayout stackLayout = new HorizontalStackLayout();

                /*// Friend profile picture
                Image friendImage = new Image
                {
                };
                friendImage.SetBinding(
                    Image.SourceProperty, "ImageSource",
                    converter: new ByteArrayToImageSourceConverter());*/

                // Friend username
                Label friendUsername = new Label
                {
                    FontSize = 20
                };
                friendUsername.SetBinding(Label.TextProperty, "FriendUserName");

                // Accept button
                Button buttonAccept = new Button
                {
                    Text = "Accept",
                    WidthRequest = 80,
                    HeightRequest = 40,
                    FontSize = 15,
                    BackgroundColor = Colors.Green
                };
                buttonAccept.HorizontalOptions = LayoutOptions.End;

                // Reject Button
                Button buttonReject = new Button
                {
                    Text = "Reject",
                    WidthRequest = 80,
                    HeightRequest = 40,
                    FontSize = 15,
                    BackgroundColor = Colors.Red
                };
                buttonReject.HorizontalOptions = LayoutOptions.End;

                //stackLayout.Children.Add(friendImage);
                stackLayout.Children.Add(friendUsername);
                stackLayout.Children.Add(buttonAccept);
                stackLayout.Children.Add(buttonReject);
                cell.View = stackLayout;

                return cell;
            });

            return listView;
        }

        private List<FriendRequest> seedFakeFriendRequests()
        {
            List<FriendRequest> pendingFriends = new List<FriendRequest>();

            for (int i = 0; i < 5; i++)
            {
                FriendRequest fakeFriend = new FriendRequest
                {
                    FriendUserName = "Test" + i,
                    FriendEmail = "Test" + i,
                    FriendStatus = 0,
                    userInitiated = true
                };
                pendingFriends.Add(fakeFriend);
            };

            return pendingFriends;
        }

        public void PopulatePopup()
        {
            List<FriendRequest> pendingFriendRequests;
            pendingFriendRequests = seedFakeFriendRequests();
            //pendingFriendRequests = await _client.

            _friendrequestView.ItemsSource = pendingFriendRequests;
        }
    }
}
