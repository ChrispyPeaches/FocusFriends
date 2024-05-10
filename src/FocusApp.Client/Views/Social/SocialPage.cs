using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Clients;
using Microsoft.Maui.Controls.Shapes;
using SimpleToolkit.SimpleShell.Extensions;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources;
using FocusApp.Client.Views.Shop;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources.FontAwesomeIcons;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Views;
using FocusCore.Queries.Social;
using Microsoft.Extensions.Logging;
using FocusCore.Models;
using MediatR;
using CommunityToolkit.Mvvm.Input;
using FocusApp.Shared.Models;
using Microsoft.Maui;

namespace FocusApp.Client.Views.Social;

internal class SocialPage : BasePage
{
    private readonly Helpers.PopupService _popupService;
    private readonly ILogger<SocialPage> _logger;
    IAuthenticationService _authenticationService;
    private IAPIClient _client;
    private IBadgeService _badgeService;
    IMediator _mediator;
    private readonly ListView _friendsListView;
    private readonly AvatarView _profilePictureNavMenuButton;

	public SocialPage(
        IAPIClient client,
        Helpers.PopupService popupService,
        IAuthenticationService authenticationService,
        IMediator mediator,
        ILogger<SocialPage> logger,
        IBadgeService badgeService
        )
	{
        _popupService = popupService;
        _client = client;
        _authenticationService = authenticationService;
        _mediator = mediator;
        _logger = logger;
        _badgeService = badgeService;

        _friendsListView = BuildFriendsListView();
        _profilePictureNavMenuButton = new AvatarView()
        {
            ImageSource = new ByteArrayToImageSourceConverter().ConvertFrom(_authenticationService.ProfilePicture)
        };

        Content = new Grid
        {
            // Define rows and columns (Star means that row/column will take up the remaining space)
            RowDefinitions = Rows.Define(80, Star, Consts.TabBarHeight),
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
                .Invoke(b => b.Clicked += OnClickShowAddFriendsPopup),

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
                _profilePictureNavMenuButton
                .Aspect(Aspect.AspectFit)
                .Right()
                .FillVertical()
                .Margins(1,1,5,1)
                .Column(1)
                // Set the corner radius to be half of the height to make it a circle
                .Bind(
                    AvatarView.CornerRadiusProperty,
                    path: nameof(AvatarView.HeightRequest),
                    convert: (double hr) => hr / 2,
                    source: RelativeBindingSource.Self)
                .BindTapGesture(
                    commandSource: this,
                    commandPath: MiscHelper.NameOf(() => TapProfilePictureShowNavigationCommand),
                    parameterPath: nameof(FriendListModel.FriendAuth0Id)),

                // Friends List
                _friendsListView
                .Row(1)
                .Column(0)
                .ColumnSpan(2),
            }
        };
    }

    private AvatarView GetProfilePictureNavMenuButton() =>
         new AvatarView()
        {
            ImageSource = new ByteArrayToImageSourceConverter().ConvertFrom(_authenticationService.ProfilePicture)
        };

    private ListView BuildFriendsListView()
    {
        ListView listView = new ListView();
        listView.IsPullToRefreshEnabled = true;
        listView.Invoke(l => l.Refreshing += RefreshFriendsList);

        listView.ItemTemplate = new DataTemplate(() =>
        {
            ViewCell cell = new ViewCell();

            Grid grid = new Grid();

            grid.RowDefinitions = Rows.Define(Star);
            grid.ColumnDefinitions = Columns.Define(80, Star);

            // Friend profile picture
            AvatarView friendImage = new AvatarView
                {
                    VerticalOptions = LayoutOptions.Center,
                }
                .Aspect(Aspect.AspectFit)
                .Column(0)
                .Bind(
                    AvatarView.ImageSourceProperty,
                    nameof(FriendListModel.FriendProfilePicture),
                    converter: new ByteArrayToImageSourceConverter())
                // Set the height of the image to be slightly smaller than the cell height
                // because padding wasn't working properly
                .Bind(
                    AvatarView.HeightRequestProperty,
                    getter: cell => cell.RenderHeight,
                    convert: (double height) => height - 1,
                    source: cell)
                // Set the width to update when the height updates and set them to the same value
                .Bind(
                    AvatarView.WidthRequestProperty,
                    path: nameof(AvatarView.HeightRequest),
                    convert: (double hr) => hr,
                    source: RelativeBindingSource.Self)
                // Set the corner radius to be half of the height to make it a circle
                .Bind(
                    AvatarView.CornerRadiusProperty,
                    path: nameof(AvatarView.HeightRequest),
                    convert: (double hr) => hr / 2,
                    source: RelativeBindingSource.Self);

            // Friend username
            Label friendUsername = new Label
            {
                FontSize = 20
            };
            friendUsername.SetBinding(Label.TextProperty, nameof(FriendListModel.FriendUserName));
            friendUsername.VerticalOptions = LayoutOptions.Center;
            friendUsername.Column(1);

            grid.Children.Add(friendImage);
            grid.Children.Add(friendUsername);
            grid.BindTapGesture(
                commandSource: this,
                commandPath: MiscHelper.NameOf(() => TapFriendItemCommand),
                parameterPath: nameof(FriendListModel.FriendAuth0Id));
            cell.View = grid;

            return cell;
        });

        return listView;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _profilePictureNavMenuButton.ImageSource = new ByteArrayToImageSourceConverter().ConvertFrom(_authenticationService?.ProfilePicture);
        
        // If not logged in display popup, otherwise populate friends list
        if (string.IsNullOrEmpty(_authenticationService.Auth0Id))
        {
            var loginPopup = (EnsureLoginPopupInterface)_popupService.ShowAndGetPopup<EnsureLoginPopupInterface>();
            loginPopup.OriginPage = nameof(SocialPage);
        }
        else
        {
            Task.Run(PopulateFriendsList);
        }
    }

    private async Task CheckForSocialBadgeEarned()
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

    // We call this function to populate FriendsList and from friends popup to refresh list
    public async Task PopulateFriendsList()
    {
        // Retrieve Friends from API
        List<FriendListModel> friendsList = new List<FriendListModel>();

        var query = new GetAllFriendsQuery
        {
            UserId = _authenticationService.Id.Value
        };

        try
        {
            friendsList = await _client.GetAllFriends(query, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while fetching friends");
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            _friendsListView.ItemsSource = friendsList;
        });



        await CheckForSocialBadgeEarned();
    }

    public RelayCommand TapProfilePictureShowNavigationCommand => new(OnClickShowProfileInterfacePopup);

    // Display navigation popup on hit
    private void OnClickShowProfileInterfacePopup()
    {
        _popupService.ShowPopup<ProfilePopupInterface>();
    }

    // Display new friend popup on hit
    private void OnClickShowAddFriendsPopup(object? sender, EventArgs e)
    {
        var addFriendPopup = (AddFriendPopupInterface)_popupService.ShowAndGetPopup<AddFriendPopupInterface>();
        addFriendPopup.SocialPage = this;
    }

    private async void RefreshFriendsList(object? sender, EventArgs e)
    {
        Task.Run(PopulateFriendsList);
        _friendsListView.EndRefresh();
    }

    public AsyncRelayCommand<string> TapFriendItemCommand => new(OnFriendClickShowFriendProfilePage);

    /// <summary>
    /// Send the user to the friend's profile page,
    /// passing the friend's Auth0Id as a parameter
    /// </summary>
    private async Task OnFriendClickShowFriendProfilePage(string? auth0Id)
        {
            Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
            await Shell.Current.GoToAsync(
                $"///{nameof(SocialPage)}/{nameof(FriendProfilePage)}",
                FriendProfilePage.BuildParamterArgs(auth0Id));
        }
}