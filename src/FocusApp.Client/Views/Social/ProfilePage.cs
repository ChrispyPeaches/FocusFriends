using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using Microsoft.Maui.Layouts;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Shared.Data;
using SimpleToolkit.SimpleShell.Extensions;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using Microsoft.EntityFrameworkCore;
using CommunityToolkit.Maui.Views;


namespace FocusApp.Client.Views.Social;

internal class ProfilePage : BasePage
{
    IAPIClient _client;
    IAuthenticationService _authenticationService;
    PopupService _popupService;
    FocusAppContext _localContext;

    // Row / Column structure for entire page
    enum PageRow { UserDataHeader, SelectedItems, MembershipDate, TabBarSpace }
    enum PageColumn { Left, Right }

    // Row / Column structure for user data header
    enum UserDataRow { UserEmail, UserName, UserPronouns }
    enum UserDataColumn { Data, EditButton }

    // Row structure for selected user items
    enum SelectedItemRow { Top, Bottom }

    #region Frontend
    public ProfilePage(IAPIClient client, IAuthenticationService authenticationService, PopupService popupService, FocusAppContext localContext) 
    {
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _localContext = localContext;

        //Shared.Models.User? localUser = _localContext.Users.First(u => u.Id == _authenticationService.CurrentUser.Id);

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
            RowDefinitions = Rows.Define(
                (PageRow.UserDataHeader, Stars(1.25)),
                (PageRow.SelectedItems, Stars(2)),
                (PageRow.MembershipDate, Stars(0.5)),
                (PageRow.TabBarSpace, Stars(0.5))
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
                .Row(PageRow.UserDataHeader)
                // When clicked, go to timer view
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    BackButtonClicked(sender, eventArgs)),

                // Profile Picture
                profilePicture
                .Column(PageColumn.Left)
                .Row(PageRow.UserDataHeader)
                .CenterVertical()
                .CenterHorizontal(),

                // Profile Info
                new Grid
                {
                    RowDefinitions = Rows.Define(
                        (UserDataRow.UserEmail, Stars(1)),
                        (UserDataRow.UserName, Stars(1.5)),
                        (UserDataRow.UserPronouns, Stars(1))
                        ),
                    ColumnDefinitions = Columns.Define(
                        (UserDataColumn.Data, Stars(2)),
                        (UserDataColumn.EditButton, Stars(0.75))
                        ),
                    Children =
                    {
                        new Label 
                        { 
                            Text = $"#{_authenticationService.CurrentUser?.Email}", 
                            FontSize = 15
                        }
                        .Row(UserDataRow.UserEmail)
                        .ColumnSpan(typeof(UserDataColumn).GetEnumNames().Length)
                        .Bottom()
                        .Left(),
                        new Label 
                        { 
                            Text = $"{_authenticationService.CurrentUser?.UserName}", 
                            FontSize = 25
                        }
                        .Row(UserDataRow.UserName)
                        .ColumnSpan(typeof(UserDataColumn).GetEnumNames().Length)
                        .CenterVertical()
                        .Left(),
                        new Label 
                        { 
                            Text = $"Pronouns: he/they{_authenticationService.CurrentUser?.Pronouns}",
                            FontSize = 15
                        }
                        .Row(UserDataRow.UserPronouns)
                        .ColumnSpan(typeof(UserDataColumn).GetEnumNames().Length-1)
                        .Top()
                        .Left(),
                        // Edit Profile Button
                        new Button
                        {
                             Text = SolidIcons.PenToSquare,
                             TextColor = Colors.Black,
                             FontFamily = nameof(SolidIcons),
                             FontSize = 20,
                             BackgroundColor = Colors.Transparent
                        }
                        .Row(UserDataRow.UserPronouns)
                        .Column(UserDataColumn.EditButton)
                        .Top()
                        // When clicked, go to timer view
                        .Invoke(button => button.Released += (sender, eventArgs) =>
                            EditButtonClicked(sender, eventArgs))
                    }
                }
                .Row(PageRow.UserDataHeader)
                .Column(PageColumn.Right),

                // Horizontal divider separating user data from selected item data
                new BoxView
                {
                    Color = Colors.Black,
                    HeightRequest = 2,
                    Opacity = 0.5
                }
                .Row(PageRow.UserDataHeader)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                .Bottom()
                .Margins(bottom: 15),


                new Grid
                { 
                    RowDefinitions = Rows.Define(
                        (SelectedItemRow.Top, Stars(1)),
                        (SelectedItemRow.Bottom, Stars(1))
                        ),
                    ColumnDefinitions = Columns.Define(
                        (PageColumn.Left, Stars(1)),
                        (PageColumn.Right, Stars(1))
                        ),
                    Children =
                    {
                        // Selected pet cell
                        new Label
                        {
                            Text = "Selected Pet",
                            FontSize = 15,
                            Opacity = 0.5
                        }
                        .Row(SelectedItemRow.Top)
                        .Column(PageColumn.Left)
                        .Top()
                        .CenterHorizontal()
                        .Margins(top: 15),
                        new Image
                        { 
                            Source = new FileImageSource { File = "pet_franklin.jpg" },
                            HeightRequest = 125,
                            WidthRequest = 125
                        }
                        .Row(SelectedItemRow.Top)
                        .Column(PageColumn.Left)
                        .Center(),
                        new Label
                        { 
                            Text = "Pet Name",
                            FontSize = 15
                        }
                        .Row(SelectedItemRow.Top)
                        .Column(PageColumn.Left)
                        .Bottom()
                        .CenterHorizontal()
                        .Margins(bottom: 15),

                        // Selected Island cell
                        new Label
                        {
                            Text = "Selected Island",
                            FontSize = 15,
                            Opacity = 0.5
                        }
                        .Row(SelectedItemRow.Top)
                        .Column(PageColumn.Right)
                        .Top()
                        .CenterHorizontal()
                        .Margins(top: 15),
                        new Image
                        {
                            Source = new FileImageSource { File = "pet_franklin.jpg" },
                            HeightRequest = 125,
                            WidthRequest = 125
                        }
                        .Row(SelectedItemRow.Top)
                        .Column(PageColumn.Right)
                        .Center(),
                        new Label
                        {
                            Text = "Island Name",
                            FontSize = 15
                        }
                        .Row(SelectedItemRow.Top)
                        .Column(PageColumn.Right)
                        .Bottom()
                        .CenterHorizontal()
                        .Margins(bottom: 15),

                        // Selected Decor cell
                        new Label
                        {
                            Text = "Selected Decor",
                            FontSize = 15,
                            Opacity = 0.5
                        }
                        .Row(SelectedItemRow.Bottom)
                        .Column(PageColumn.Left)
                        .Top()
                        .CenterHorizontal()
                        .Margins(top: 15),
                        new Image
                        {
                            Source = new FileImageSource { File = "pet_franklin.jpg" },
                            HeightRequest = 125,
                            WidthRequest = 125
                        }
                        .Row(SelectedItemRow.Bottom)
                        .Column(PageColumn.Left)
                        .Center(),
                        new Label
                        {
                            Text = "Decor Name",
                            FontSize = 15
                        }
                        .Row(SelectedItemRow.Bottom)
                        .Column(PageColumn.Left)
                        .Bottom()
                        .CenterHorizontal()
                        .Margins(bottom: 15),

                        // Selected Badge cell
                        new Label
                        {
                            Text = "Selected Badge",
                            FontSize = 15,
                            Opacity = 0.5
                        }
                        .Row(SelectedItemRow.Bottom)
                        .Column(PageColumn.Right)
                        .Top()
                        .CenterHorizontal()
                        .Margins(top: 15),
                        new Image
                        {
                            Source = new FileImageSource { File = "pet_franklin.jpg" },
                            HeightRequest = 125,
                            WidthRequest = 125
                        }
                        .Row(SelectedItemRow.Bottom)
                        .Column(PageColumn.Right)
                        .Center(),
                        new Label
                        {
                            Text = "Badge Name",
                            FontSize = 15
                        }
                        .Row(SelectedItemRow.Bottom)
                        .Column(PageColumn.Right)
                        .Bottom()
                        .CenterHorizontal()
                        .Margins(bottom: 15),

                        // Dividers for the grid sections
                        new BoxView
                        {
                            Color = Colors.Black,
                            HeightRequest = 2,
                            Opacity = 0.5
                        }
                        .Row(SelectedItemRow.Top)
                        .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                        .Bottom()
                        .Margins(left: 50, right: 50),
                        new BoxView
                        {
                            Color = Colors.Black,
                            WidthRequest = 2,
                            Opacity = 0.5
                        }
                        .RowSpan(typeof(SelectedItemRow).GetEnumNames().Length)
                        .Column(PageColumn.Left)
                        .Right()
                        .Margins(top: 50, bottom: 50)
                        //.Margins(top: 60, bottom: 60)
                    } 
                    
                }
                .Row(PageRow.SelectedItems)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length),

                // Date Joined
                new Label
                {
                    Text = $"Member Since: {_authenticationService.CurrentUser?.DateCreated.ToShortDateString()}",
                    TextColor = Colors.Black,
                    FontSize = 20
                }
                .Row(PageRow.MembershipDate)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                .CenterVertical()
                .CenterHorizontal()
            }
        };
    }

    private async void EditButtonClicked(object sender, EventArgs eventArgs)
    {
        Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
        await Shell.Current.GoToAsync(nameof(ProfilePageEdit));
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
