using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Views.Shop;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using Microsoft.Maui.Layouts;


namespace FocusApp.Client.Views.Social;

internal class UserBadgesPage : BasePage
{
    IAPIClient _client;
    IAuthenticationService _authenticationService;
    FocusAppContext _localContext;
    Helpers.PopupService _popupService;


    #region Frontend
    public UserBadgesPage(IAPIClient client, IAuthenticationService authenticationService, Helpers.PopupService popupService, FocusAppContext localContext)
    {
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _localContext = localContext;

        BackgroundColor = AppStyles.Palette.OrchidPink;

        var flexLayout = new FlexLayout
        {
            Direction = FlexDirection.Row,
            Wrap = FlexWrap.Wrap,
            AlignItems = FlexAlignItems.Start,
            JustifyContent = FlexJustify.SpaceBetween
        };

        var badges = GetLocalBadges();

        foreach (var badge in badges)
        {
            //var badgeView = new BadgeView(badge);
        }
    }
    #endregion

    #region Backend

    protected override async void OnAppearing()
    {
        // Theoretically a user shouldnt be able to get here, but added for redundancy
        if (_authenticationService.CurrentUser == null)
        {
            var loginPopup = (EnsureLoginPopupInterface)_popupService.ShowAndGetPopup<EnsureLoginPopupInterface>();
            loginPopup.OriginPage = nameof(ShopPage);
        }

        // TODO: Add logic to fetch from local db

        base.OnAppearing();
    }

    private List<UserBadge> GetLocalBadges()
    {
        return null;
    }
    #endregion
}
