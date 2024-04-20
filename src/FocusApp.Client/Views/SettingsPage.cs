using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using CommunityToolkit.Maui.Views;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Resources;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using Microsoft.Maui.Layouts;
using SimpleToolkit.SimpleShell.Extensions;

namespace FocusApp.Client.Views;

internal sealed class SettingsPage : BasePage
{
    IAPIClient _client { get; set; }

    public SettingsPage(IAPIClient client)
    {
        _client = client;

        // Default values for preferences
        bool isStartupTipsEnabled = PreferencesHelper.Get<bool>(PreferencesHelper.PreferenceNames.startup_tips_enabled);
        bool isSessionRatingEnabled = PreferencesHelper.Get<bool>(PreferencesHelper.PreferenceNames.session_rating_enabled);

        // Using grids
        Content = new Grid
        {
            // Define the length of the rows & columns
            RowDefinitions = Rows.Define(80, 70, 70, 70, 70, Star, GridRowsColumns.Stars(2), Star),
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
                
                // Show Mindful Tips on Startup
                new Label
                    {
                        Text = "Show Tips on Startup",
                        TextColor = Colors.Black,
                        FontSize = 30
                    }
                    .Row(1)
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
                    .Row(1)
                    .Column(5)
                    .Left()
                    .CenterVertical()
                    .Invoke(sw => sw.Toggled += (sender, e) => { PreferencesHelper.Set(PreferencesHelper.PreferenceNames.startup_tips_enabled, e.Value); }),
                
                
                // Show Session Rating
                new Label
                    {
                        Text = "Show Session Rating",
                        TextColor = Colors.Black,
                        FontSize = 30
                    }
                    .Row(2)
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
                    .Row(2)
                    .Column(5)
                    .Left()
                    .CenterVertical()
                    .Invoke(sw => sw.Toggled += (sender, e) => { PreferencesHelper.Set(PreferencesHelper.PreferenceNames.session_rating_enabled, e.Value); }),
                
                
                // Tutorial
                new Label
                    {
                        Text = "Tutorial", 
                        TextDecorations = TextDecorations.Underline,
                        TextColor = Colors.Black,
                        FontSize = 30
                    }
                    .Row(3)
                    .Column(0)
                    .CenterVertical()
                    .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                    .ColumnSpan(3),

                // Tutorial Button
                new Button
                    {
                        Opacity = 0
                    }
                    .Row(3)
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
                .Row(4)
                .Column(0)
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3),
                
                // About Button
                new Button
                {
                    Opacity = 0
                }
                .Row(4)
                .Column(0)
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(2)
                .Invoke(b => b.Clicked += (sender, e) => {Console.WriteLine("About Button Tapped");}),

                new Image()
                {
                    Source = "logo.png",
                }
                .FillVertical()
                .Aspect(Aspect.AspectFit)
                .Row(6)
                .ColumnSpan(5)
            }
        };
    }

    

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }
}
