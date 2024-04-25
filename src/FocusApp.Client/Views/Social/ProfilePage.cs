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

internal class ProfilePage : BasePage
{
    IAPIClient _client;
    IAuthenticationService _authenticationService;
    PopupService _popupService;
    FocusAppContext _localContext;

    // Row / Column structure for entire page
    enum PageRow { UserProfilePictureHeader, SelectedItems, UserDataFooter, MembershipDate, TabBarSpace }
    enum PageColumn { Left, Right }

    // Row / Column structure for user data header
    enum UserDataRow { UserName, UserPronouns, UserEmail }
    enum UserDataColumn { Data, UtilityButtons }

    // Row structure for selected user items
    enum SelectedItemRow { Top, Bottom }

    // User data references
    AvatarView _profilePicture { get; set; }
    Label _userName { get; set; }
    Label _pronouns { get; set; }
    Label _email { get; set; }

    // Selected user item references
    ImageButton _selectedPet { get; set; }
    ImageButton _selectedIsland { get; set; }
    ImageButton _selectedDecor { get; set; }
    ImageButton _selectedBadge { get; set; }
    #region Frontend
    public ProfilePage(IAPIClient client, IAuthenticationService authenticationService, PopupService popupService, FocusAppContext localContext) 
    {
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _localContext = localContext;

        CreateUserDataElements();
        CreateSelectedItemElements();

        Content = new Grid
        {
            RowDefinitions = Rows.Define(
                (PageRow.UserProfilePictureHeader, Stars(0.75)),
                (PageRow.SelectedItems, Stars(2)),
                (PageRow.UserDataFooter, Stars(0.75)),
                (PageRow.MembershipDate, Stars(0.35)),
                (PageRow.TabBarSpace, Stars(0.5))
                ),
            ColumnDefinitions = Columns.Define(
                (PageColumn.Left, Stars(1)),
                (PageColumn.Right, Stars(1))
                ),
            BackgroundColor = AppStyles.Palette.Celeste,

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

                // Edit Profile Button
                new Button
                {
                        Text = SolidIcons.PenToSquare,
                        TextColor = Colors.Black,
                        FontFamily = nameof(SolidIcons),
                        FontSize = 30,
                        BackgroundColor = Colors.Transparent
                }
                .Row(PageRow.UserProfilePictureHeader)
                .Column(PageColumn.Right)
                .Top()
                .Right()
                // When clicked, go to edit profile view
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    EditButtonClicked(sender, eventArgs)),

                // Profile Picture
                _profilePicture
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                .Row(PageRow.UserProfilePictureHeader)
                .Center(),

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
                        /* Selected pet cell */
                        new Label
                        {
                            Text = "Selected Pet",
                            FontSize = 15,
                            Opacity = 0.5
                        }
                        .Row(SelectedItemRow.Top)
                        .Column(PageColumn.Left)
                        .Bottom()
                        .CenterHorizontal()
                        .Margins(bottom: 15),

                        // Selected pet image button
                        _selectedPet?
                        .Row(SelectedItemRow.Top)
                        .Column(PageColumn.Left)
                        .Center(),

                        new Label
                        {
                            Text = $"{
                                (_authenticationService.CurrentUser?.SelectedPet?.Name == null
                                ? "Select a pet!"
                                : _authenticationService.CurrentUser?.SelectedPet?.Name)}",
                            FontSize = 15
                        }
                        .Row(SelectedItemRow.Top)
                        .Column(PageColumn.Left)
                        .Top()
                        .CenterHorizontal()
                        .Margins(top: 15),

                        /* Selected Island cell */
                        new Label
                        {
                            Text = "Selected Island",
                            FontSize = 15,
                            Opacity = 0.5
                        }
                        .Row(SelectedItemRow.Top)
                        .Column(PageColumn.Right)
                        .Bottom()
                        .CenterHorizontal()
                        .Margins(bottom: 15),

                        // Selected island image button
                        _selectedIsland?
                        .Row(SelectedItemRow.Top)
                        .Column(PageColumn.Right)
                        .Center(),

                        new Label
                        {
                            Text = $"{
                                (_authenticationService.CurrentUser?.SelectedIsland?.Name == null
                                ? "Select an island!"
                                : _authenticationService.CurrentUser?.SelectedIsland?.Name)}",
                            FontSize = 15
                        }
                        .Row(SelectedItemRow.Top)
                        .Column(PageColumn.Right)
                        .Top()
                        .CenterHorizontal()
                        .Margins(top:15),

                        /* Selected Decor cell */
                        new Label
                        {
                            Text = "Selected Decor",
                            FontSize = 15,
                            Opacity = 0.5
                        }
                        .Row(SelectedItemRow.Bottom)
                        .Column(PageColumn.Left)
                        .Bottom()
                        .CenterHorizontal()
                        .Margins(bottom: 15),

                        // Selected decor image button
                        _selectedDecor?
                        .Row(SelectedItemRow.Bottom)
                        .Column(PageColumn.Left)
                        .Center(),

                        new Label
                        {
                            Text = $"{
                                (_authenticationService.CurrentUser?.SelectedDecor?.Name == null
                                ? "Select decor!"
                                : _authenticationService.CurrentUser?.SelectedDecor?.Name)}",
                            FontSize = 15
                        }
                        .Row(SelectedItemRow.Bottom)
                        .Column(PageColumn.Left)
                        .Top()
                        .CenterHorizontal(),

                        /* Selected Badge cell */
                        new Label
                        {
                            Text = "Selected Badge",
                            FontSize = 15,
                            Opacity = 0.5
                        }
                        .Row(SelectedItemRow.Bottom)
                        .Column(PageColumn.Right)
                        .Bottom()
                        .CenterHorizontal()
                        .Margins(bottom: 15),
                        
                        // Selected badge image button
                        _selectedBadge?
                        .Row(SelectedItemRow.Bottom)
                        .Column(PageColumn.Right)
                        .Center(),

                        new Label
                        {
                            Text = $"{
                                (_authenticationService.CurrentUser?.SelectedBadge?.Name == null
                                ? "Select a badge!"
                                : _authenticationService.CurrentUser?.SelectedBadge?.Name)}",
                            FontSize = 15
                        }
                        .Row(SelectedItemRow.Bottom)
                        .Column(PageColumn.Right)
                        .Top()
                        .CenterHorizontal(),

                        // Dividers for the selected item grid sections
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
                    }
                }
                .Row(PageRow.SelectedItems)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length),

                // Horizontal divider separating user data from selected item data
                new BoxView
                {
                    Color = Colors.Black,
                    HeightRequest = 2,
                    Opacity = 0.5
                }
                .Row(PageRow.UserDataFooter)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                .Top(),

                // Profile Info
                new Grid
                {
                    RowDefinitions = Rows.Define(
                        (UserDataRow.UserName, Stars(1.5)),
                        (UserDataRow.UserPronouns, Stars(1)),
                        (UserDataRow.UserEmail, Stars(1))
                        ),
                    ColumnDefinitions = Columns.Define(
                        (UserDataColumn.Data, Stars(1)),
                        (UserDataColumn.UtilityButtons, Stars(0.25))
                        ),
                    Children =
                    {
                        new Button
                        {
                             Text = SolidIcons.Copy,
                             TextColor = Colors.Black,
                             FontFamily = nameof(SolidIcons),
                             FontSize = 20,
                             BackgroundColor = Colors.Transparent
                        }
                        .Row(UserDataRow.UserEmail)
                        .Column(UserDataColumn.UtilityButtons)
                        .Right()
                        .CenterVertical()
                        .Invoke(button => button.Released += (sender, eventArgs) =>
                            CopyEmailClicked(sender, eventArgs)),

                         _userName
                        .Row(UserDataRow.UserName)
                        .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                        .CenterVertical()
                        .Left()
                        .Margins(left: 15),

                        _pronouns
                        .Row(UserDataRow.UserPronouns)
                        .ColumnSpan(typeof(UserDataColumn).GetEnumNames().Length-1)
                        .CenterVertical()
                        .Left()
                        .Margins(left: 15),

                        _email
                        .Row(UserDataRow.UserEmail)
                        .ColumnSpan(typeof(UserDataColumn).GetEnumNames().Length-1)
                        .CenterVertical()
                        .Left()
                        .Margins(left: 15),
                    }
                }
                .Row(PageRow.UserDataFooter)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length),

                // Date Joined
                new Label
                {
                    Text = $"Member Since: {_authenticationService.CurrentUser?.DateCreated.ToLocalTime().ToShortDateString()}",
                    TextColor = Colors.Black,
                    FontSize = 20
                }
                .Row(PageRow.MembershipDate)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                .CenterVertical()
                .CenterHorizontal(),
            }
        };
    }

    // Refresh bindings on page load in case of profile edit / navigation to settings page
    protected override async void OnAppearing()
    {
        await AppShell.Current.SetTabBarIsVisible(false);

        ByteArrayToImageSourceConverter byteArrayConverter = new ByteArrayToImageSourceConverter();

        // Set user details bindings
        _profilePicture.ImageSource = byteArrayConverter.ConvertFrom(_authenticationService.CurrentUser?.ProfilePicture);
        _userName.Text = _authenticationService.CurrentUser?.UserName;
        _pronouns.Text = $"Pronouns: {_authenticationService.CurrentUser?.Pronouns}";
        _email.Text = $"Friend Id: #{_authenticationService.CurrentUser?.Email}";

        // Set user selected items bindings
        _selectedPet.Source = byteArrayConverter.ConvertFrom(_authenticationService.SelectedPet?.Image);
        _selectedIsland.Source = byteArrayConverter.ConvertFrom(_authenticationService.SelectedIsland?.Image);

        if (_authenticationService.SelectedDecor != null)
            _selectedDecor.Source = byteArrayConverter.ConvertFrom(_authenticationService.SelectedDecor?.Image);

        if (_authenticationService.SelectedBadge != null)
            _selectedBadge.Source = byteArrayConverter.ConvertFrom(_authenticationService.SelectedBadge?.Image);
    }


    private void CreateUserDataElements()
    {
        // Set bindable properties with images
        _profilePicture = new AvatarView
        {
            CornerRadius = 60,
            HeightRequest = 120,
            WidthRequest = 120
        }
        .Bind(AvatarView.ImageSourceProperty,
              "ProfilePicture",
              converter: new ByteArrayToImageSourceConverter());
        _profilePicture.BindingContext = _authenticationService.CurrentUser;

        _userName = new Label
        {
            Text = $"{_authenticationService.CurrentUser?.UserName}",
            FontSize = 18
        };

        _pronouns = new Label
        {
            Text = $"Pronouns: {_authenticationService.CurrentUser?.Pronouns}",
            FontSize = 15
        };

        _email = new Label
        {
            Text = $"Friend Id: #{_authenticationService.CurrentUser?.Email}",
            FontSize = 15
        };
    }

    private void CreateSelectedItemElements()
    {
        // Selected item labels
        _selectedPetLabel = new Label
        {
            Text = $"{(_authenticationService.CurrentUser?.SelectedPet?.Name == null
                                ? "Select a pet!"
                                : _authenticationService.CurrentUser?.SelectedPet?.Name)}",
            FontSize = 15
        };

        _selectedIslandLabel = new Label
        {
            Text = $"{(_authenticationService.CurrentUser?.SelectedIsland?.Name == null
                                ? "Select an island!"
                                : _authenticationService.CurrentUser?.SelectedIsland?.Name)}",
            FontSize = 15
        };

        // Selected item image buttons
        _selectedPet = new ImageButton
        {
            HeightRequest = 110,
            WidthRequest = 110
        }
        .Bind(ImageButton.SourceProperty, "Image", converter: new ByteArrayToImageSourceConverter())
        .Invoke(button => button.Released += (sender, eventArgs) =>
            SelectedPetClicked(sender, eventArgs));

        _selectedIsland = new ImageButton
        {
            HeightRequest = 110,
            WidthRequest = 110
        }
        .Bind(ImageButton.SourceProperty, "Image", converter: new ByteArrayToImageSourceConverter())
        .Invoke(button => button.Released += (sender, eventArgs) =>
            SelectedIslandClicked(sender, eventArgs));

        _selectedDecor = new ImageButton
        {
            HeightRequest = 110,
            WidthRequest = 110
        }
        .Bind(ImageButton.SourceProperty, "Image", converter: new ByteArrayToImageSourceConverter())
        .Invoke(button => button.Released += (sender, eventArgs) =>
            SelectedDecorClicked(sender, eventArgs));

        _selectedBadge = new ImageButton
        {
            HeightRequest = 110,
            WidthRequest = 110
        }
        .Bind(ImageButton.SourceProperty, "Image", converter: new ByteArrayToImageSourceConverter())
        .Invoke(button => button.Released += (sender, eventArgs) =>
            SelectedBadgeClicked(sender, eventArgs));
    }

    #endregion

    #region Backend
    private async void CopyEmailClicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(_authenticationService.CurrentUser?.Email);
    }

    private async void EditButtonClicked(object sender, EventArgs eventArgs)
    {
        Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(ProfilePageEdit)}");
    }

    private async void SelectedPetClicked(object sender, EventArgs eventArgs)
    {
        Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(PetsPage)}");
    }

    private async void SelectedIslandClicked(object sender, EventArgs eventArgs)
    {
        Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(PetsPage)}");
    }

    private async void SelectedDecorClicked(object sender, EventArgs eventArgs)
    {
        Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(PetsPage)}");
    }

    private async void SelectedBadgeClicked(object sender, EventArgs eventArgs)
    {
        Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(PetsPage)}");
    }

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        // Back navigation reverses animation so can keep right to left
        Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
    }

    #endregion
}
