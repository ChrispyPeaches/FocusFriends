using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Clients;
using FocusCore.Queries.User;
using SimpleToolkit.SimpleShell.Extensions;
using Microsoft.Maui.Storage;

namespace FocusApp.Client.Views;

internal sealed class SettingsPage : BasePage
{
    IAPIClient _client { get; set; }
    public SettingsPage(IAPIClient client)
    {
        _client = client;

        // Defualt values for preferences
        double sfxVolume = Preferences.Get("sfx_volume", 50.00);
        double ambianceVolume = Preferences.Get("ambiance_volume", 50.00);
        var isNotificationsEnabled = Preferences.Get("notifications_enabled", false);
        var isDarkmodeEnabled = Preferences.Get("darkmode_enabled", false);

        // Using grids
        Content = new Grid
        {
            // Define the lenth of the rows & columns
            RowDefinitions = Rows.Define(80, 70, 70, 70, 70, 70, 70, Star),
            ColumnDefinitions = Columns.Define(Star, Star, Star, Star, Star),

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


                // SFX
                new Label
                {
                    Text = "SFX",
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(1)
                .Column(0)
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .CenterVertical()
                .ColumnSpan(3),

                // SFX Slider
                new Slider
                {
                    Maximum = 100,
                    Value = sfxVolume,
                    WidthRequest = 200,
                }
                .Row(1)
                .Column(2)
                .CenterVertical()
                .ColumnSpan(3)
                // When the value is changed, save it & print for debug
                //.Invoke(s => s.ValueChanged += (sender, e) => {sfxVolume = e.NewValue; Console.WriteLine($"SFX volume is {sfxVolume})");}),
                .Invoke(s => s.ValueChanged += (sender, e) => {Preferences.Set("sfx_volume",e.NewValue);}),


                // Ambiance
                new Label
                {
                    Text = "Ambiance",
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(2)
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
                .Row(2)
                .Column(2)
                .CenterVertical()
                .ColumnSpan(3)
                // When the value is changed, save it & print for debug
                //.Invoke(s => s.ValueChanged += (sender, e) => {ambianceVolume = e.NewValue; Console.WriteLine($"New volume is {ambianceVolume})");}),
                .Invoke(s => s.ValueChanged += (sender, e) => {Preferences.Set("ambiance_volume", e.NewValue);}),


                // Notifications
                new Label
                {
                    Text = "Notifications",
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(3)
                .Column(0)
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3),

                // Notifications Switch
                new Switch
                {
                    ThumbColor = Colors.SlateGrey,
                    OnColor = Colors.Green,
                    IsToggled = Preferences.Get("notifications_enabled", false)
                }
                .Row(3)
                .Column(3)
                .Left()
                .CenterVertical()
                //.Invoke(sw => sw.Toggled += (sender, e) => { Console.WriteLine("Notifications Switch Tapped"); }),
                .Invoke(sw => sw.Toggled += (sender, e) => { SaveSwitchState("notifications_enabled", e.Value); }),

                // Dark Mode
                new Label
                {
                    Text = "Dark Mode",
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(4)
                .Column(0)
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3),

                // Dark Mode Switch
                new Switch
                {
                    ThumbColor = Colors.SlateGrey,
                    OnColor = Colors.Green,
                    IsToggled = Preferences.Get("darkmode_enabled", false)
                }
                .Row(4)
                .Column(3)
                .Left()
                .CenterVertical()
                //.Invoke(sw => sw.Toggled += (sender, e) => { Console.WriteLine("Dark Mode Switch Tapped"); }),
                .Invoke(sw => sw.Toggled += (sender, e) => { SaveSwitchState("darkmode_enabled", e.Value); }),


                // Languages
                new Label
                {
                    Text = "Languages",
                    TextDecorations = TextDecorations.Underline,
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(5)
                .Column(0)
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3),

                // There should be a way to transform this later into a hyperlink
                // Invisible button for languages functionality
                new Button
                {
                    Opacity = 0
                }
                .Row(5)
                .Column(0)
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3)
                // When the button is pressed, print for debug
                .Invoke(b => b.Clicked += (sender, e) => {Console.WriteLine("Languages Button Tapped");}),


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

                // There should be a way to transform this later into a hyperlink
                // Invisible button for languages functionality
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

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        // Back navigation reverses animation so can keep right to left
        Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
        await Shell.Current.GoToAsync("..");
    }

    private void SaveSwitchState(string key, bool val)
    {
        Preferences.Set(key, val);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }
}
