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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace FocusApp.Client.Views.Social;

internal class BadgesPage : BasePage
{
    private readonly IAPIClient _client;
    private readonly IAuthenticationService _authenticationService;
    private readonly PopupService _popupService;
    private readonly FocusAppContext _localContext;
    private readonly ILogger<BadgesPage> _logger;

    public Image _selectedCheckmark;

    public Guid SelectedBadgeId { get; set; }

    public FlexLayout _flexLayout;
    public Dictionary<Badge, Image> BadgeDict { get; set; }

    public BadgesPage(IAPIClient client, IAuthenticationService authenticationService, PopupService popupService, FocusAppContext localContext, ILogger<BadgesPage> logger)
    {
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _localContext = localContext;
        _logger = logger;

        // FlexLayout for the Badges
        _flexLayout = new FlexLayout
        {
            Direction = FlexDirection.Row,
            Wrap = FlexWrap.Wrap,
            JustifyContent = FlexJustify.SpaceAround
        };

        Content = new Grid
        {
            RowDefinitions = Rows.Define(80, 10, Star, 80),
            ColumnDefinitions = Columns.Define(Star, Star),
            BackgroundColor = AppStyles.Palette.DarkMauve,

            Children =
            {
            // Header
            new Label
            {
                Text = "Badges",
                TextColor = Colors.Black,
                FontSize = 40
            }
            .Row(0)
            .Column(0)
            .Center()
            .ColumnSpan(2)
            .CenterVertical()
            .Paddings(top: 5, bottom: 5, left: 5, right: 5),

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
            BackButtonClicked(sender, eventArgs)),

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
            .ColumnSpan(5),

            new ScrollView
            {
               Content = _flexLayout
            }
            // Badges FlexLayout
            .ColumnSpan(2)
            .Row(2)
            }
        };

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Fetch badges from the local database and display them
        var localBadges = await FetchBadgesFromLocalDb();
        var userBadges = await FetchUserBadgesFromLocalDb();
        DisplayBadges(localBadges, userBadges);
    }

    // Becuse all of our badges are synced on start, we fetch them to diplay all
    private async Task<List<Badge>> FetchBadgesFromLocalDb()
    {
        var badges = new List<Badge>();
        try
        {
            // Fetch badges from the local database
            badges = await _localContext.Badges.ToListAsync();
        }
        catch (Exception ex)
        { 
            _logger.LogError(ex, "Error when fetching badges from local DB.");
        }
        return badges;
    }

    // User badges are fetched to be compared with the badges on local to differenciate owned and unowned
    private async Task<List<Guid>> FetchUserBadgesFromLocalDb()
    {
        var userBadgeIds = new List<Guid>();
        try
        {
            userBadgeIds =  await _localContext.UserBadges?
                .Where(ub => ub.UserId == _authenticationService.Id.Value)
                .Select(ub => ub.BadgeId).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when fetching user badges from local DB.");
        }
        return userBadgeIds;
    }

    private void DisplayBadges(List<Badge> localBadges, List<Guid> userBadgeIds)
    {
        var flexLayout = _flexLayout;

        _flexLayout.Children.Clear();
        BadgeDict = new Dictionary<Badge, Image>();
        // Add badges to FlexLayout
        foreach (var badge in localBadges)
        {
            var isOwned = userBadgeIds.Any(userBadgeId => userBadgeId == badge.Id);

            var checkmark = new Image 
            {
                Source = "pet_selected.png",
                WidthRequest = 60,
                HeightRequest = 60,
                Opacity = 0,
                BindingContext = badge
            };

            if (isOwned)
            {
                BadgeDict.Add(badge, checkmark);
                if (_authenticationService.SelectedBadge?.Id == badge.Id)
                {
                    _selectedCheckmark = checkmark;
                    _selectedCheckmark.Opacity = 1;
                }

                var ownedBadge = new ImageButton
                {
                    BindingContext = badge,
                    Source = ImageSource.FromStream(() => new MemoryStream(badge.Image)),
                    Aspect = Aspect.AspectFit,
                    WidthRequest = 150,
                    HeightRequest = 150
                }
                .Invoke(button => button.Released += (s, e) => OnImageButtonClicked(s, e));

                var ownedGrid = new Grid
                {
                    ownedBadge
                    .ZIndex(0),

                    checkmark
                    .ZIndex(1)
                };
                flexLayout.Children.Add(ownedGrid);
            }
            else
            {
                var unownedBadge = new ImageButton
                {
                    BindingContext = badge,
                    Source = ImageSource.FromStream(() => new MemoryStream(badge.Image)),
                    Aspect = Aspect.AspectFit,
                    WidthRequest = 150,
                    HeightRequest = 150,
                    Opacity = .2
                }
                .Invoke(button => button.Released += (s, e) => OnImageButtonClicked(s, e));

                var unownedGrid = new Grid
                {
                    unownedBadge
                    .ZIndex(0)
                };
                flexLayout.Children.Add(unownedGrid);

            }
        }
    }

    void OnImageButtonClicked(object sender, EventArgs eventArgs)
    {
        var itemButton = sender as ImageButton;
        var badge = (Badge)itemButton.BindingContext;

        var itemPopup = (BadgesPagePopupInterface)_popupService.ShowAndGetPopup<BadgesPagePopupInterface>();
        itemPopup.UserBadgesPage = this;
        itemPopup.PopulatePopup(badge);
    }

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        // Back navigation reverses animation so can keep right to left
        Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
    }
}
