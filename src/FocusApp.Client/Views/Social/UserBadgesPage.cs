using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Views.Shop;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Layouts;

namespace FocusApp.Client.Views.Social;

internal class UserBadgesPage : BasePage
{
    private readonly IAPIClient _client;
    private readonly IAuthenticationService _authenticationService;
    private readonly PopupService _popupService;
    private readonly FocusAppContext _localContext;

    public UserBadgesPage(IAPIClient client, IAuthenticationService authenticationService, PopupService popupService, FocusAppContext localContext)
    {
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _localContext = localContext;

        BackgroundColor = AppStyles.Palette.OrchidPink;

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
        var badges = await FetchBadgesFromLocalDb();
        DisplayBadges(badges);
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

    private void DisplayBadges(List<Badge> badges)
    {
        // Configure FlexLayout for badge display
        var flexLayout = new FlexLayout
        {
            Direction = FlexDirection.Row,
            Wrap = FlexWrap.Wrap,
            JustifyContent = FlexJustify.SpaceAround
        };

        // Add badges to FlexLayout
        foreach (var badge in badges)
        {
            var badgeImage = new Image
            {
                Source = ImageSource.FromStream(() => new MemoryStream(badge.Image)),
                Aspect = Aspect.AspectFit,
                WidthRequest = 100, 
                HeightRequest = 100 
            };

            flexLayout.Children.Add(badgeImage);
        }

        // Set the FlexLayout as the content of the page
        Content = flexLayout;
    }
}
