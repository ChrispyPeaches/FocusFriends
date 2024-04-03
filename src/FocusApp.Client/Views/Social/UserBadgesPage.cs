using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Shared.Data;


namespace FocusApp.Client.Views.Social;

internal class UserBadgesPage : BasePage
{
    IAPIClient _client;
    IAuthenticationService _authenticationService;
    FocusAppContext _focusAppContext;
    Helpers.PopupService _popupService;


    #region Frontend
    public UserBadgesPage(IAPIClient client, IAuthenticationService authenticationService, Helpers.PopupService popupService, FocusAppContext localContext)
    {
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _focusAppContext = localContext;

        Content = new FlexLayout
        {
            BackgroundColor = AppStyles.Palette.FairyTale,
            Children =
            {

            }
        };
    }
    #endregion

    #region Backend
    #endregion
}
