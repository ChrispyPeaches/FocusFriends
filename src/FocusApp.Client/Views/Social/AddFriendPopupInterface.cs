using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusCore.Commands.Social;
using FocusCore.Models;
using FocusCore.Queries.Social;
using FocusCore.Responses.Social;
using Microsoft.Maui.Controls.Shapes;
using Refit;
using System.Net;
using Microsoft.Extensions.Logging;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using CommunityToolkit.Maui.Core;
using FocusApp.Client.Resources.FontAwesomeIcons;

namespace FocusApp.Client.Views.Social
{
    internal class AddFriendPopupInterface : BasePopup
    {
        private readonly IAPIClient _client;
        private readonly IAuthenticationService _authenticationService;
        private readonly Helpers.PopupService _popupService;
        private readonly IBadgeService _badgeService;
        private readonly ILogger<AddFriendPopupInterface> _logger;

        private readonly ListView _friendrequestView;
        private readonly Label entryError;
        public SocialPage SocialPage { get; set; }

        public AddFriendPopupInterface(
            IAPIClient client,
            IAuthenticationService authenticationService,
            Helpers.PopupService popupService,
            IBadgeService badgeService,
            ILogger<AddFriendPopupInterface> logger)
        {
            _client = client;
            _popupService = popupService;
            _authenticationService = authenticationService;
            _badgeService = badgeService;
            _logger = logger;

            Closed += AddFriendPopupInterface_Closed;

            _friendrequestView = BuildFriendRequestListView();

            Color = Colors.Transparent;

            CanBeDismissedByTappingOutsideOfPopup = false;

            Entry emailEntry = new Entry
            {
                Placeholder = "Enter friend's email",
                FontSize = 20,
                HeightRequest = 50,
                WidthRequest = 240,
                TextColor = Colors.Black,
                ClearButtonVisibility = ClearButtonVisibility.Never,
                Keyboard = Keyboard.Plain,
                VerticalOptions = LayoutOptions.Start
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
                Content = new Grid
                {
                    // Define rows and columns
                    RowDefinitions = Rows.Define(55, 60, Auto, Auto, Star),
                    ColumnDefinitions = Columns.Define(Star),

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
                            Content = new Grid()
                            {
                                WidthRequest = 360,
                                HeightRequest = 55,

                                // Define rows and columns
                                RowDefinitions = Rows.Define(Star),
                                ColumnDefinitions = Columns.Define(Star, Star, Star),

                                Children =
                                {
                                    new Label()
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
                                    .Row(0)
                                    .Column(1),

                                    // Dismiss Popup Button
                                    new Button
                                    {
                                        Text = SolidIcons.x,
                                        TextColor = Colors.White,
                                        FontFamily = nameof(SolidIcons),
                                        FontSize = 20,
                                        BackgroundColor = Colors.Transparent
                                    }
                                    .ZIndex(2)
                                    .Right()
                                    .Paddings(top: 10, bottom: 10, right: 25)
                                    .Column(2)
                                    .Row(0)
                                    // When clicked, close the popup
                                    .Invoke(button => button.Released += OnDismissPopup),
                                }
                            }
                        }
                        .Column(0)
                        .Row(0),

                        new Frame()
                        {
                            WidthRequest = 360,
                            HeightRequest = 60,
                            Padding = new Thickness(20, 20, 20, 0),
                            BackgroundColor = Colors.Transparent,
                            Content = new HorizontalStackLayout
                            {
                                Spacing = 0,
                                Children =
                                {
                                    new Label
                                    {
                                        Text = SolidIcons.Hashtag,
                                        TextColor = AppStyles.Palette.DarkMauve,
                                        FontFamily = nameof(SolidIcons),
                                        FontSize = 20,
                                        BackgroundColor = Colors.Transparent
                                    }
                                    .CenterVertical(),

                                    emailEntry,

                                    new Button
                                    {
                                        Text = SolidIcons.CircleArrowRight,
                                        FontFamily = nameof(SolidIcons),
                                        WidthRequest = 63,
                                        HeightRequest = 45,
                                        FontSize = 20,
                                        BindingContext = emailEntry,
                                        VerticalOptions = LayoutOptions.Start,
                                        BackgroundColor = AppStyles.Palette.DarkMauve
                                    }
                                    .Invoke(b => b.Clicked += (s,e) => OnClickSendFriendRequest(s,e))
                                }
                            }
                            .Margins(top: -15)
                        }
                        .Column(0)
                        .Row(1),

                        // Error label for friend request errors
                        entryError
                        .Top()
                        .Paddings(left: 15, bottom: 5)
                        .Column(0)
                        .Row(2),

                        // Horizontal Divider
                        new BoxView
                        {
                            Color = AppStyles.Palette.DarkMauve,
                            WidthRequest = 360,
                            HeightRequest = 2
                        }
                        .Top()
                        .Column(0)
                        .Row(3),

                        new Frame()
                        {
                            WidthRequest = 360,
                            HeightRequest = 350,
                            BackgroundColor = Colors.Transparent,
                            Content = _friendrequestView
                        }
                        .Column(0)
                        .Row(4),
                    }
                }
                .Top()
                .Right()
            };

            // Populate PendingFriends upon popup open
            PopulatePopup();
        }

        private void AddFriendPopupInterface_Closed(object? sender, PopupClosedEventArgs e)
        {
            _popupService.HidePopup(wasDismissedByTappingOutsideOfPopup: true);
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
                    FontSize = 12
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

        private async void PopulatePopup()
        {
            List<FriendRequest> pendingFriendRequests;

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
                    entryError.Text = "Friendship already exists or is pending";
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

            // Return if entry is empty
            if (friendEmail == null || friendEmail == "")
            {
                entryError.Text = "Please enter an email address";
                return;
            }

            // Return if user enters their own email
            if (friendEmail == _authenticationService.Email)
            {
                entryError.Text = "Please don't enter your own email";
                return;
            }

            var friendRequest = new CreateFriendRequestCommand
            {
                UserEmail = _authenticationService.Email,
                FriendEmail = friendEmail
            };

            // Create Friend Request
            ApiResponse<CreateFriendRequestResponse>? response = await _client.CreateFriendRequest(friendRequest);

            // Populate error label if necessary
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

            // Accept Friend Request
            await _client.AcceptFriendRequest(acceptCommand);

            // Refresh friends list
            Task.Run(SocialPage.PopulateFriendsList);
            Task.Run(ShowSocialBadgeIfEarned);

            PopulatePopup();

            
        }

        private async Task ShowSocialBadgeIfEarned()
        {
            try
            {
                BadgeEligibilityResult result = await _badgeService.CheckSocialBadgeEligibility();
                if (result is { IsEligible: true, EarnedBadge: not null })
                {
                    _popupService.TriggerBadgeEvent<EarnedBadgePopupInterface>(result.EarnedBadge);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking for badge eligibility.");
            }
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

            // Reject Friend Request
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

            // Cancel Friend Request
            await _client.CancelFriendRequest(cancelCommand);

            PopulatePopup();
        }

        // Navigate to page according to button
        private async void OnDismissPopup(object? sender, EventArgs e)
        {
            _popupService.HidePopup();
        }
    }
}
