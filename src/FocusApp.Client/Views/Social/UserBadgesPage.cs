using CommunityToolkit.Maui.Converters;
using FocusApp.Client;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Views.Shop;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Layouts;
using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources.FontAwesomeIcons;
using CommunityToolkit.Maui.Markup.LeftToRight;
using SimpleToolkit.SimpleShell.Extensions;

namespace FocusApp.Client.Views.Social;

internal class UserBadgesPage : BasePage
{
    private readonly IAPIClient _client;
    private readonly IAuthenticationService _authenticationService;
    private readonly PopupService _popupService;
    private readonly FocusAppContext _localContext;

    public Guid SelectedBadgeId { get; set; }

    public UserBadgesPage(IAPIClient client, IAuthenticationService authenticationService, PopupService popupService, FocusAppContext localContext)
    {
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _localContext = localContext;

        var grid = new Grid
        {
            RowDefinitions = Rows.Define(80,10,Star,70),
            ColumnDefinitions = Columns.Define(Star,Star),
            BackgroundColor = AppStyles.Palette.DarkMauve,
        };
        grid.Children.Add(
            // Header
            new Label
            {
                Text = "Badges",
                TextColor = Colors.Black,
                FontSize = 40
            }
            .Row(0)
            .Column(1)
            .Left()
            .ColumnSpan(2)
            .CenterVertical()
            .Paddings(top: 5, bottom: 5, left: 5, right: 5));

        grid.Children.Add(
            // Back Button
            new Button
            {
                Text = SolidIcons.ChevronLeft,
                TextColor = Colors.Black,
                FontFamily = nameof(SolidIcons),
                FontSize = 40,
                BackgroundColor = Colors.Transparent
            }
            .Left()
            .CenterVertical()
            .Paddings(top: 10, bottom: 10, left: 15, right: 15)
            .Column(0)
            .Invoke(button => button.Released += (sender, eventArgs) =>
            BackButtonClicked(sender, eventArgs)));

        grid.Children.Add(
            // Header & Content Divider
            new BoxView
            {
                Color = Color.Parse("Black"),
                WidthRequest = 400,
                HeightRequest = 2,
            }
            .Bottom()
            .Row(0)
            .Column(0)
            .ColumnSpan(5));

        // Configure FlexLayout for badge display
        grid.Children.Add(new FlexLayout
        {
            Direction = FlexDirection.Row,
            Wrap = FlexWrap.Wrap,
            JustifyContent = FlexJustify.SpaceAround
        }
        .Row(2)
        .ColumnSpan(2));

        Content = grid;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Check if the user is logged in
        if (_authenticationService.CurrentUser == null)
        {
            var loginPopup = (EnsureLoginPopupInterface)_popupService.ShowAndGetPopup<EnsureLoginPopupInterface>();
            loginPopup.OriginPage = nameof(ShopPage);
            return;
        }

        // Fetch badges from the local database and display them
        var localBadges = await FetchBadgesFromLocalDb();
        // Replace below with API when complete
        var userBadges = await FetchBadgesFromLocalDb();
        DisplayBadges(localBadges, userBadges);
    }

    private async Task<List<Badge>> FetchBadgesFromLocalDb()
    {
        try
        {
            // Fetch badges from the local database
            return await _localContext.Badges.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error when fetching badges from local DB.", ex);
        }
    }

    /*
    private async Task<List<Badge?>> FetchBadgesFromAPI()
    {
        try
        {
            var userId = _authenticationService.CurrentUser?.Id;

            return await _authenticationService.CurrentUser.Badges
                .Where(badge => badge.UserId == userId)
                .Select(badge => badge.Badge)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error when fetching badges from API.", ex);
        }
    }
    */

    private void DisplayBadges(List<Badge> localBadges, List<Badge> userBadges)
    {
        var flexLayout = (FlexLayout)((Grid)Content).Children[3];

        // Add badges to FlexLayout
        foreach (var badge in localBadges)
        {
            var isOwned = userBadges.Any(userBadge => userBadge.Id == badge.Id);

            if (isOwned)
            {
                var ownedBadge = new ImageButton
                {
                    Source = ImageSource.FromStream(() => new MemoryStream(badge.Image)),
                    Aspect = Aspect.AspectFit,
                    WidthRequest = 150,
                    HeightRequest = 150
                };
                flexLayout.Children.Add(ownedBadge);
            }
            else
            {
                var unownedBadge = new Image
                {
                    Source = ImageSource.FromStream(() => new MemoryStream(badge.Image)),
                    Aspect = Aspect.AspectFit,
                    WidthRequest = 150,
                    HeightRequest = 150,
                    Opacity = .2
                };
                flexLayout.Children.Add(unownedBadge);
            }
                
        }
        // Set the FlexLayout as the content of the page
        //Content = flexLayout;
    }

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        // Back navigation reverses animation so can keep right to left
        Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
    }
}
