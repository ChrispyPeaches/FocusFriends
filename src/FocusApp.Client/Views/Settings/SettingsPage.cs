using Auth0.OidcClient;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Resources;
using FocusApp.Client.Helpers;
using CommunityToolkit.Mvvm.Input;

namespace FocusApp.Client.Views.Settings;

internal sealed class SettingsPage : BasePage
{
    private PopupService _popupService;
    private readonly Auth0Client _auth0Client;
    private readonly IAuthenticationService _authService;

    private bool _isLoggedIn;
    private bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => SetProperty(ref _isLoggedIn, value);
    }

    private enum Row { Header, StartupMindfulnessTipSetting, ShowSessionRatingSetting, AboutButton, LoginLogoutButton, Whitespace1, Logo, Whitespace2 }

    public SettingsPage(
        PopupService popupService,
        IAuthenticationService authService,
        Auth0Client auth0Client)
    {
        _popupService = popupService;
        _authService = authService;
        _auth0Client = auth0Client;

        // Default values for preferences
        bool isStartupTipsEnabled = PreferencesHelper.Get<bool>(PreferencesHelper.PreferenceNames.startup_tips_enabled);
        bool isSessionRatingEnabled = PreferencesHelper.Get<bool>(PreferencesHelper.PreferenceNames.session_rating_enabled);

        // Using grids
        Content = new Grid
        {
            // Define the length of the rows & columns
            RowDefinitions = Rows.Define(
                (Row.Header, 80),
                (Row.StartupMindfulnessTipSetting, 70),
                (Row.ShowSessionRatingSetting, 70),
                (Row.AboutButton, 70),
                (Row.LoginLogoutButton, 70),
                (Row.Whitespace1, Star),
                (Row.Logo, Stars(2)),
                (Row.Whitespace2, Star)),
            ColumnDefinitions = Columns.Define(Star, Star, Star, Star, Star),
            BackgroundColor = AppStyles.Palette.LightPeriwinkle,

            Children =
            {
                // Header
                new Label
                {
                    Text = "Settings",
                    TextColor = Colors.Black,
                    FontSize = 40
                }
                .Row(Row.Header)
                .Column(1)
                .ColumnSpan(3)
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
                // When clicked, go to timer view
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
                .Row(Row.Header)
                .Column(0)
                .ColumnSpan(5),
                
                // Show Mindful Tips on Startup
                new Label
                    {
                        Text = "Show Tips on Startup",
                        TextColor = Colors.Black,
                        FontSize = 25
                    }
                    .Row(Row.StartupMindfulnessTipSetting)
                    .Column(0)
                    .CenterVertical()
                    .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                    .ColumnSpan(5),

                // Show Mindful Tips on Startup Switch
                new Switch
                    {
                        ThumbColor = Colors.SlateGrey,
                        OnColor = Colors.Green,
                        IsToggled = isSessionRatingEnabled
                    }
                    .Row(Row.StartupMindfulnessTipSetting)
                    .Column(5)
                    .Left()
                    .CenterVertical()
                    .Invoke(sw => sw.Toggled += (sender, e) => 
                        PreferencesHelper.Set(PreferencesHelper.PreferenceNames.startup_tips_enabled, e.Value)),
                
                
                // Show Session Rating
                new Label
                    {
                        Text = "Show Session Rating",
                        TextColor = Colors.Black,
                        FontSize = 25
                    }
                    .Row(Row.ShowSessionRatingSetting)
                    .Column(0)
                    .CenterVertical()
                    .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                    .ColumnSpan(5),

                // Show Session Rating Switch
                new Switch
                    {
                        ThumbColor = Colors.SlateGrey,
                        OnColor = Colors.Green,
                        IsToggled = isStartupTipsEnabled
                    }
                    .Row(Row.ShowSessionRatingSetting)
                    .Column(5)
                    .Left()
                    .CenterVertical()
                    .Invoke(sw => sw.Toggled += (sender, e) => 
                        PreferencesHelper.Set(PreferencesHelper.PreferenceNames.session_rating_enabled, e.Value)),

                // About
                new Label
                {
                    Text = "About",
                    TextDecorations = TextDecorations.Underline,
                    TextColor = Colors.Black,
                    FontSize = 25
                }
                .Row(Row.AboutButton)
                .Column(0)
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3)
                .BindTapGesture(
                    commandSource: this,
                    commandPath: MiscHelper.NameOf(() => TapAboutCommand)),

                // Logout
                new Label
                    {
                        Text = IsLoggedIn ? "Logout" : "Login",
                        TextDecorations = TextDecorations.Underline,
                        TextColor = Colors.Black,
                        FontSize = 25
                    }
                    .Row(Row.LoginLogoutButton)
                    .Column(0)
                    .CenterVertical()
                    .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                    .ColumnSpan(3)
                    .Bind(Label.TextProperty,
                        getter: static (settingsPage) => settingsPage.IsLoggedIn,
                        convert: static (bool isLoggedIn) => isLoggedIn ? "Logout" : "Login",
                        source: this)
                    .BindTapGesture(
                        commandSource: this,
                        commandPath: MiscHelper.NameOf(() => TapLoginLogoutButtonCommand)),

                // Logo
                new Image()
                {
                    Source = "logo_with_border.png",
                }
                .FillVertical()
                .Aspect(Aspect.AspectFit)
                .Row(Row.Logo)
                .ColumnSpan(5)
            }
        };
    }

    public AsyncRelayCommand TapAboutCommand => new(OnTapAboutButton, AsyncRelayCommandOptions.None);

    private async Task OnTapAboutButton()
    {
        _popupService.ShowPopup<SettingsAboutPopupInterface>();
    }

    public AsyncRelayCommand TapLoginLogoutButtonCommand => new(OnTapLoginLogoutButton, AsyncRelayCommandOptions.None);

    private async Task OnTapLoginLogoutButton()
    {
        if (IsLoggedIn)
        {
            await _authService.Logout(_auth0Client);
        }
        else
        {
            await Shell.Current.GoToAsync("///" + nameof(LoginPage));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        IsLoggedIn = !string.IsNullOrEmpty(_authService.Auth0Id);
    }
}
