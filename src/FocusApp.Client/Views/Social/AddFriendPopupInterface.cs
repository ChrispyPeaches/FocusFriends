using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using CommunityToolkit.Maui.Views;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Shared.Models;
using FocusCore.Commands.Social;
using FocusCore.Queries.Social;
using FocusCore.Responses.Social;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics.Text;
using Refit;
using SimpleToolkit.SimpleShell.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace FocusApp.Client.Views.Social
{
    internal class AddFriendPopupInterface : BasePopup
    {
        IAPIClient _client;
        IAuthenticationService _authenticationService;
        Helpers.PopupService _popupService;
        ListView _friendrequestView { get; set; }
        Label entryError {  get; set; }
        public SocialPage SocialPage { get; set; }

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
                WidthRequest = 240,
                TextColor = Colors.Black,
                ClearButtonVisibility = ClearButtonVisibility.Never,
                Keyboard = Keyboard.Plain
            };

            entryError = new Label
            {
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Red,
                FontSize = 15
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
                                        WidthRequest = 80,
                                        HeightRequest = 50,
                                        FontSize = 20,
                                        BindingContext = emailEntry
                                    }
                                    .Invoke(b => b.Clicked += (s,e) => OnClickSendFriendRequest(s,e))
                                }
                            }
                        },

                        // Error label for friend request errors
                        entryError
                        .Paddings(left: 15)
                        .Top(),

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
                Grid grid = new Grid();

                grid.RowDefinitions = Rows.Define(Star);
                grid.ColumnDefinitions = Columns.Define(Star, 80, 80);

                // Friend username
                Label friendUsername = new Label
                {
                    FontSize = 15
                };
                friendUsername.SetBinding(Label.TextProperty, "FriendUserName");
                friendUsername.VerticalOptions = LayoutOptions.Center;
                friendUsername.Column(0);

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
                buttonAccept.VerticalOptions = LayoutOptions.Center;
                buttonAccept.Column(1);
                buttonAccept.Invoke(b => b.Clicked += (s, e) => OnClickAcceptFriendRequest(s, e));

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
                buttonReject.VerticalOptions = LayoutOptions.Center;
                buttonReject.Column(2);
                buttonReject.Invoke(b => b.Clicked += (s, e) => OnClickRejectFriendRequest(s, e));

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
                buttonCancel.VerticalOptions = LayoutOptions.Center;
                buttonCancel.Column(1,2);
                buttonCancel.Invoke(b => b.Clicked += (s, e) => OnClickCancelFriendRequest(s, e));

                grid.Children.Add(friendUsername);
                grid.Children.Add(buttonAccept);
                grid.Children.Add(buttonReject);
                grid.Children.Add(buttonCancel);
                cell.View = grid;

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

        // Populate entry error label with corresponding message
        public void PopulateErrorLabel(HttpStatusCode httpCode)
        {
            switch (httpCode)
            {
                case HttpStatusCode.Conflict:
                    entryError.Text = "You are already friends with this user";
                    break;
                case HttpStatusCode.NotFound:
                    entryError.Text = "Invalid user email";
                    break;
                case HttpStatusCode.InternalServerError:
                    entryError.Text = "Server error, try again later";
                    break;
                default:
                    entryError.Text = "Error, try again later";
                    break;
            }
        }

        private async void OnClickSendFriendRequest(object sender, EventArgs e)
        {
            // Clear entry error
            entryError.Text = "";

            var sendButton = sender as Button;
            var emailEntry = (Entry)sendButton.BindingContext;

            var friendEmail = emailEntry.Text;

            var friendRequest = new CreateFriendRequestCommand
            {
                UserEmail = _authenticationService.CurrentUser.Email,
                FriendEmail = friendEmail
            };

            ApiResponse<CreateFriendRequestResponse>? response = await _client.CreateFriendRequest(friendRequest);

            var httpCode = response.StatusCode;

            if (httpCode != HttpStatusCode.OK)
            {
                PopulateErrorLabel((HttpStatusCode)httpCode);
            }

            PopulatePopup();
        }

        private async void OnClickAcceptFriendRequest(object sender, EventArgs e)
        {
            var acceptButton = sender as Button;
            var friendRequest = (FriendRequest)acceptButton.BindingContext;

            var friendId = friendRequest.FriendId;

            var acceptCommand = new AcceptFriendRequestCommand
            {
                UserId = _authenticationService.CurrentUser.Id,
                FriendId = friendId 
            };

            await _client.AcceptFriendRequest(acceptCommand);

            // Refresh friends list
            SocialPage.RepopulateFriendsList();

            PopulatePopup();
        }

        private async void OnClickRejectFriendRequest(object sender, EventArgs e)
        {
            var cancelButton = sender as Button;
            var friendRequest = (FriendRequest)cancelButton.BindingContext;

            var friendId = friendRequest.FriendId;

            var cancelCommand = new CancelFriendRequestCommand
            {
                UserId = friendId,
                FriendId = _authenticationService.CurrentUser.Id
            };

            await _client.CancelFriendRequest(cancelCommand);

            PopulatePopup();
        }

        private async void OnClickCancelFriendRequest(object sender, EventArgs e)
        {
            var cancelButton = sender as Button;
            var friendRequest = (FriendRequest)cancelButton.BindingContext;

            var friendId = friendRequest.FriendId;

            var cancelCommand = new CancelFriendRequestCommand
            {
                UserId = _authenticationService.CurrentUser.Id,
                FriendId = friendId
            };

            await _client.CancelFriendRequest(cancelCommand);

            PopulatePopup();
        }
    }
}
