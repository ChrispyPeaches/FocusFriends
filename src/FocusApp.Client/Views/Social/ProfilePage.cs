using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Shared.Data;
using SimpleToolkit.SimpleShell.Extensions;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace FocusApp.Client.Views.Social;

internal class ProfilePage : BasePage
{
    IAPIClient _client;
    IAuthenticationService _authenticationService;
    PopupService _popupService;
    FocusAppContext _localContext;

    #region Frontend
    public ProfilePage(IAPIClient client, IAuthenticationService authenticationService, PopupService popupService, FocusAppContext localContext) 
    {
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _localContext = localContext;

        // Using grids
        Content = new Grid
        {
            // Define the length of the rows & columns
            RowDefinitions = Rows.Define(),
            ColumnDefinitions = Columns.Define(),
            BackgroundColor = AppStyles.Palette.LightMauve,

            Children =
            {
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
                // When clicked, go to timer view
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    BackButtonClicked(sender, eventArgs)),
            }
        };
    }

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        // Back navigation reverses animation so can keep right to left
        Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
    }
    #endregion

    #region Backend
    #endregion
}
