using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Resources;
using FocusApp.Client.Clients;
using SimpleToolkit.SimpleShell.Extensions;

namespace FocusApp.Client.Views;

internal sealed class SettingsPage : BasePage
{
    IAPIClient _client { get; set; }
    public SettingsPage(IAPIClient client)
    {
        _client = client;

        // Default values for preferences
        double ambianceVolume = Preferences.Default.Get("ambiance_volume", 50.00);
        var isNotificationsEnabled = Preferences.Default.Get("notifications_enabled", false);
        var isStartupTipsEnabled = Preferences.Default.Get("startup_tips_enabled", true);
        var isSessionRatingEnabled = Preferences.Default.Get("session_rating_enabled", true);
        
        // Using grids
        Content = new Grid
        {
            // Define the length of the rows & columns
            RowDefinitions = Rows.Define(80, 70, 70, 70, 70, 70, 70, Star),
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
                .Row(0)
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
                .Row(0)
                .Column(0)
                .ColumnSpan(5),


                // Ambiance
                new Label
                {
                    Text = "Ambiance",
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(1)
                .Column(0)
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3),

                // Ambiance Slider
                new Slider
                {
                    Maximum = 100,
                    Value = ambianceVolume,
                    WidthRequest = 200
                }
                .Row(1)
                .Column(2)
                .CenterVertical()
                .ColumnSpan(3)
                .Invoke(s => s.ValueChanged += (sender, e) => {Preferences.Default.Set("ambiance_volume", e.NewValue);}),


                // Notifications
                new Label
                {
                    Text = "Notifications",
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(2)
                .Column(0)
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3),

                // Notifications Switch
                new Switch
                {
                    ThumbColor = Colors.SlateGrey,
                    OnColor = Colors.Green,
                    IsToggled = isNotificationsEnabled
                }
                .Row(2)
                .Column(5)
                .Left()
                .CenterVertical()
                .Invoke(sw => sw.Toggled += (sender, e) => { SaveSwitchState("notifications_enabled", e.Value); }),
                
                
                // Show Mindful Tips on Startup
                new Label
                    {
                        Text = "Show Tips on Startup",
                        TextColor = Colors.Black,
                        FontSize = 30
                    }
                    .Row(3)
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
                    .Row(3)
                    .Column(5)
                    .Left()
                    .CenterVertical()
                    .Invoke(sw => sw.Toggled += (sender, e) => { SaveSwitchState("startup_tips_enabled", e.Value); }),
                
                
                // Show Session Rating
                new Label
                    {
                        Text = "Show Session Rating",
                        TextColor = Colors.Black,
                        FontSize = 30
                    }
                    .Row(4)
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
                    .Row(4)
                    .Column(5)
                    .Left()
                    .CenterVertical()
                    .Invoke(sw => sw.Toggled += (sender, e) => { SaveSwitchState("session_rating_enabled", e.Value); }),
                
                
                // Tutorial
                new Label
                    {
                        Text = "Tutorial", 
                        TextDecorations = TextDecorations.Underline,
                        TextColor = Colors.Black,
                        FontSize = 30
                    }
                    .Row(5)
                    .Column(0)
                    .CenterVertical()
                    .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                    .ColumnSpan(3),

                // Tutorial Button
                new Button
                    {
                        Opacity = 0
                    }
                    .Row(5)
                    .Column(0)
                    .CenterVertical()
                    .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                    .ColumnSpan(2)
                    .Invoke(b => b.Clicked += (sender, e) => {Console.WriteLine("Tutorial Button Tapped");}),
                

                // About
                new Label
                {
                    Text = "About", 
                    TextDecorations = TextDecorations.Underline,
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(6)
                .Column(0)
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3),
                
                // About Button
                new Button
                {
                    Opacity = 0
                }
                .Row(6)
                .Column(0)
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(2)
                .Invoke(b => b.Clicked += (sender, e) => {Console.WriteLine("About Button Tapped");}),

                
                // Logo
                new Image
                {
                    Source = "logo.png"
                }
                .Row(7)
                .Column(2)
                .Center()
            }
        };
    }

    

    private void SaveSwitchState(string key, bool val)
    {
        Preferences.Default.Set(key, val);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }
}
