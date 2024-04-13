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
using CommunityToolkit.Maui.Views;

namespace FocusApp.Client.Views.Social;

internal class ProfilePageEdit : BasePage
{

    IAPIClient _client;
    IAuthenticationService _authenticationService;
    PopupService _popupService;
    FocusAppContext _localContext;

    // Page Row / Column Definitions
    enum PageRow { ProfilePicture, FormFields, TabBarSpace }
    enum PageColumn { Left, Right }

    #region Frontend
    public ProfilePageEdit(IAPIClient client, IAuthenticationService authenticationService, PopupService popupService, FocusAppContext localContext)
    {
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _localContext = localContext;

        // Set bindable properties with images
        AvatarView profilePicture = new AvatarView
        {
            CornerRadius = 63,
            HeightRequest = 126,
            WidthRequest = 126
        }
        .Bind(AvatarView.ImageSourceProperty, "ProfilePicture", converter: new ByteArrayToImageSourceConverter());
        profilePicture.BindingContext = _authenticationService.CurrentUser;

        Content = new Grid
        {
            // Define the length of the rows & columns
            RowDefinitions = Rows.Define(
                (PageRow.ProfilePicture, Stars(0.75)),
                (PageRow.FormFields, Stars(2)),
                (PageRow.TabBarSpace, Stars(0.25))
                ),
            ColumnDefinitions = Columns.Define(
                (PageColumn.Left, Stars(1)),
                (PageColumn.Right, Stars(1))
                ),
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
                .Top()
                // When clicked, go to timer view
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    BackButtonClicked(sender, eventArgs)),

                // Profile Picture
                profilePicture
                .Row(PageRow.ProfilePicture)
                .Column(PageColumn.Right)
                .Center(),

                new Button
                { 
                    Text = SolidIcons.Plus,
                    TextColor = Colors.Black,
                    FontSize = 30,
                    Opacity = 0.35,
                    CornerRadius = 63,
                    WidthRequest = 126,
                    HeightRequest = 126,
                    BackgroundColor = Colors.LightGray
                }
                .Row(PageRow.ProfilePicture)
                .Column(PageColumn.Right)
                .Center(),

                // Profile Info
                new StackLayout
                {
                    Spacing = 50,
                    Children =
                    {
                        new Label { Text = $"#{_authenticationService.CurrentUser?.Email}", FontSize = 30 },
                        new Entry { Placeholder = "Username" },
                        new Entry { Placeholder = "Pronouns" },
                        new Entry { Placeholder = "First Name" },
                        new Entry { Placeholder = "Last Name" },
                    }
                }
                .Row(PageRow.FormFields)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                .Left()
                .FillHorizontal(),
            }
        };
    }

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        // Back navigation reverses animation so can keep right to left
        Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(ProfilePage)}");
    }
    #endregion

    #region Backend
    #endregion
}
