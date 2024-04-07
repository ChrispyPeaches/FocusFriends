using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using CommunityToolkit.Maui.Views;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Shared.Models;
using FocusCore.Commands.Social;
using FocusCore.Models;
using FocusCore.Queries.Social;
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
        IAuthenticationService _authenticationService;
        Helpers.PopupService _popupService;
        ListView _friendrequestView { get; set; }

        public AddFriendPopupInterface(IAPIClient client, IAuthenticationService authenticationService, Helpers.PopupService popupService)
        {
            _client = client;
            _popupService = popupService;
            _authenticationService = authenticationService;

            _friendrequestView = BuildFriendRequestListView();

            Color = Colors.Transparent;

            Entry emailEntry = new Entry
            {
                Placeholder = "Enter friend's email",
                FontSize = 20,
                TextColor = Colors.Black,
                ClearButtonVisibility = ClearButtonVisibility.Never,
                Keyboard = Keyboard.Plain
            };

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
                                    emailEntry.Fill(),

                                    new Button
                                    {
                                        Text = "Send",
                                        WidthRequest = 100,
                                        HeightRequest = 50,
                                        FontSize = 20,
                                        BindingContext = emailEntry
                                    }
                                    .Invoke(b => b.Clicked += (s,e) => OnClickSendFriendRequest(s,e))
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

                // Friend profile picture
                Image friendImage = new Image
                {
                };
                friendImage.SetBinding(
                    Image.SourceProperty, "ImageSource",
                    converter: new ByteArrayToImageSourceConverter());

                // Friend username
                Label friendUsername = new Label
                {
                    FontSize = 20
                };
                friendUsername.SetBinding(Label.TextProperty, "FriendUserName");

                // Accept button (Invitee Only)
                Button buttonAccept = new Button
                {
                    Text = "Accept",
                    WidthRequest = 80,
                    HeightRequest = 40,
                    FontSize = 15,
                    BackgroundColor = Colors.Green,
                };
                buttonAccept.SetBinding(Button.IsVisibleProperty, "UserInitiated", converter: new InvertedBoolConverter());
                buttonAccept.HorizontalOptions = LayoutOptions.End;

                // Reject Button (Invitee Only)
                Button buttonReject = new Button
                {
                    Text = "Reject",
                    WidthRequest = 80,
                    HeightRequest = 40,
                    FontSize = 15,
                    BackgroundColor = Colors.Red
                };
                buttonReject.SetBinding(Button.IsVisibleProperty, "UserInitiated", converter: new InvertedBoolConverter());
                buttonReject.HorizontalOptions = LayoutOptions.End;

                // Cancel Button (Inviter Only)
                Button buttonCancel = new Button
                {
                    Text = "Cancel",
                    WidthRequest = 80,
                    HeightRequest = 40,
                    FontSize = 15,
                    BackgroundColor = Colors.Red
                };
                buttonCancel.SetBinding(Button.IsVisibleProperty, "UserInitiated");
                buttonCancel.HorizontalOptions = LayoutOptions.End;

                //stackLayout.Children.Add(friendImage);
                stackLayout.Children.Add(friendUsername);
                stackLayout.Children.Add(buttonAccept);
                stackLayout.Children.Add(buttonReject);
                stackLayout.Children.Add(buttonCancel);
                cell.View = stackLayout;

                return cell;
            });

            return listView;
        }

        private List<FriendRequest> seedFakeFriendRequests()
        {
            List<FriendRequest> pendingFriends = new List<FriendRequest>();

            for (int i = 0; i < 10; i++)
            {
                bool init = true;
                if (i % 2 == 0)
                {
                    init = false;
                }

                FriendRequest fakeFriend = new FriendRequest
                {
                    FriendUserName = "Test" + i,
                    FriendEmail = "Test" + i,
                    UserInitiated = init,
                };
                pendingFriends.Add(fakeFriend);
            };

            return pendingFriends.OrderBy(pf => pf.UserInitiated).ToList();
        }

        public async void PopulatePopup()
        {
            List<FriendRequest> pendingFriendRequests;
            //pendingFriendRequests = seedFakeFriendRequests();

            // Fetch all pending friend requests
            var query = new GetAllFriendRequestsQuery
            {
                UserId = _authenticationService.CurrentUser.Id
            };
            pendingFriendRequests = await _client.GetAllFriendRequests(query, default);

            _friendrequestView.ItemsSource = pendingFriendRequests;
        }

        private void OnClickSendFriendRequest(object sender, EventArgs e)
        {
            var sendButton = sender as Button;
            var emailEntry = (Entry)sendButton.BindingContext;

            var friendEmail = emailEntry.Text;

            var friendRequest = new CreateFriendRequestCommand
            {
                UserEmail = _authenticationService.CurrentUser.Email,
                FriendEmail = friendEmail
            };

            _client.CreateFriendRequest(friendRequest);
            
        }
        private void OnClickAcceptFriendRequest(object sender, EventArgs e)
        {
            var acceptButton = sender as Button;
            var friendRequest = (FriendRequest)acceptButton.BindingContext;

            var friendId = friendRequest.FriendId;

            var acceptCommand = new AcceptFriendRequestCommand
            {
                UserId = _authenticationService.CurrentUser.Id,
                FriendId = friendId 
            };

            _client.AcceptFriendRequest(acceptCommand);

        }
    }
}
