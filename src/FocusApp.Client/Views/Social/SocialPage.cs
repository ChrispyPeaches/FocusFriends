using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Clients;
using FocusCore.Queries.User;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SimpleToolkit.SimpleShell.Extensions;
using Microsoft.Maui.Platform;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources;
using FocusApp.Client.Views.Shop;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources.FontAwesomeIcons;
using CommunityToolkit.Maui.Converters;
using Microsoft.Maui.ApplicationModel;
using FocusCore.Queries.Social;
using Microsoft.Extensions.Logging;
using FocusCore.Models;
using FocusApp.Shared.Models;
using MediatR;

namespace FocusApp.Client.Views.Social;

internal class SocialPage : BasePage
{
    private Helpers.PopupService _popupService;
    private readonly ILogger<SocialPage> _logger;
    IAuthenticationService _authenticationService;
    private IMediator _mediator;
    public ListView _friendsListView;

    IAPIClient _client { get; set; }

	public SocialPage(
        IAPIClient client,
        Helpers.PopupService popupService,
        IAuthenticationService authenticationService,
        IMediator mediator,
        ILogger<SocialPage> logger
        )
	{
        _popupService = popupService;
        _client = client;
        _authenticationService = authenticationService;
        _mediator = mediator;
        _logger = logger;
        _logger = logger;

        _friendsListView = BuildFriendsListView();

        Content = new Grid
        {
            // Define rows and columns (Star means that row/column will take up the remaining space)
            RowDefinitions = Rows.Define(80, Star),
            ColumnDefinitions = Columns.Define(Star, Star),
            BackgroundColor = AppStyles.Palette.Celeste,
            Opacity = 0.9,

            Children =
            {
                // Header
                new Label
                {
                    TextColor = Colors.Black,
                    Text = "Friends",
                    FontSize = 40
                }
                .Row(0)
                .Column(0)
                .Padding(15, 15),

                new Button
                {
                    BackgroundColor = Colors.Transparent,
                    FontFamily = nameof(SolidIcons),
                    TextColor = Colors.Black,
                    Text = SolidIcons.PersonCirclePlus,
                    FontSize = 35
                }
                .Row(0)
                .Column(1)
                .Left()
                .Padding(15, 15)
                .Invoke(b => b.Clicked += (s,e) => OnClickShowAddFriendsPopup(s,e)),

                // Horizontal Divider
                new BoxView
                {
                    Color = Colors.Black,
                    WidthRequest = 400,
                    HeightRequest = 2
                }
                .Bottom()
                .Row(0)
                .Column(0)
                .ColumnSpan(2),

                // Profile Picture
                new ImageButton
                {
                    Source = new FileImageSource
                    {
                        // Add logic that gets profile picture from user data
                        File = "dotnet_bot.png"
                    },
                    WidthRequest = 90,
                    HeightRequest = 90
                }
                .Top()
                .Right()
                .Column(1)
                .Clip(new EllipseGeometry { Center = new Point(43, 45), RadiusX = 27, RadiusY = 27 })
                .Invoke(b => b.Clicked += (s,e) => OnClickShowProfileInterfacePopup(s,e)),

                // Friends List
                _friendsListView
                .Row(1)
                .Column(0)
                .ColumnSpan(2),
                new Button()
                    .Row(2).Column(0)
                    .Invoke(b => b.Clicked += OnFriendClickShowFriendProfilePage)
            }
        };
    }

    private ListView BuildFriendsListView()
    {
        ListView listView = new ListView();

        listView.ItemTemplate = new DataTemplate(() =>
        {
            ViewCell cell = new ViewCell();
            Grid grid = new Grid();

            grid.RowDefinitions = Rows.Define(Star);
            grid.ColumnDefinitions = Columns.Define(80, Star);

            // Friend profile picture
            Image friendImage = new Image
            {
            };
            friendImage.SetBinding(
                Image.SourceProperty, "FriendProfilePicture",
                converter: new ByteArrayToImageSourceConverter());
            friendImage.VerticalOptions = LayoutOptions.Center;
            friendImage.Column(0);

            // Friend username
            Label friendUsername = new Label
            {
                FontSize = 20
            };
            friendUsername.SetBinding(Label.TextProperty, "FriendUserName");
            friendUsername.VerticalOptions = LayoutOptions.Center;
            friendUsername.Column(1);

            grid.Children.Add(friendImage);
            grid.Children.Add(friendUsername);
            cell.View = grid;

            return cell;
        });

        return listView;
    }

    protected override async void OnAppearing()
    {
        // If not logged in display popup, otherwise populate friends list
        if (string.IsNullOrEmpty(_authenticationService.AuthToken))
        {
            var loginPopup = (EnsureLoginPopupInterface)_popupService.ShowAndGetPopup<EnsureLoginPopupInterface>();
            loginPopup.OriginPage = nameof(SocialPage);
        }
        else
        {
            PopulateFriendsList();
        }

        base.OnAppearing();
    }

    // We call this function to populate FriendsList and from friends popup to refresh list
    public async void PopulateFriendsList()
    {
        // Retrieve Friends from API
        List<FriendListModel> friendsList = new List<FriendListModel>();

        var query = new GetAllFriendsQuery
        {
            UserId = _authenticationService.CurrentUser.Id
        };

        try
        {
            friendsList = await _client.GetAllFriends(query, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while fetching friends");
        }

        _friendsListView.ItemsSource = friendsList;
    }

    // Display navigation popup on hit
    private void OnClickShowProfileInterfacePopup(object sender, EventArgs e)
    {
        _popupService.ShowPopup<ProfilePopupInterface>();
    }

    // Display new friend popup on hit
    private void OnClickShowAddFriendsPopup(object sender, EventArgs e)
    {
        var addFriendPopup = (AddFriendPopupInterface)_popupService.ShowAndGetPopup<AddFriendPopupInterface>();
        addFriendPopup.SocialPage = this;
    }
}