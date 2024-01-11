using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Resources.FontAwesomeIcons;

namespace FocusApp.Views;

internal sealed class SettingsView : ContentView
{
    public SettingsView()
    {
        double value = 50;
        // Using grids
        Content = new Grid
        {
            // Define the lenth of the rows & columns
            RowDefinitions = Rows.Define(80, 70, 70, 70, 70, 70, Star),
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

                // Button
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
                .Invoke(b => b.Clicked += (sender, e) => {Console.WriteLine("Clicked");}),

                // Divider
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

                // Settings:

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
                .ColumnSpan(3)
                ,

                // SFX Slider
                new Slider
                {
                    Maximum = 100,
                    Value = value,
                    WidthRequest = 200,
                }
                .Invoke(s => s.ValueChanged += (sender, e) => {value = e.NewValue; Console.WriteLine($"New value is {value})");})
                .Row(1)
                .Column(2)
                .ColumnSpan(3)
                ,

                // Ambiance
                new Label
                {
                    Text = "Ambiance",
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(2)
                .Column(0)
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3)
                ,

                // Notifications
                new Label
                {
                    Text = "Notifications",
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(3)
                .Column(0)
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3)
                ,

                // Dark Mode
                new Label
                {
                    Text = "Dark Mode",
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(4)
                .Column(0)
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3)
                ,

                // Languages
                new Label
                {
                    Text = "Languages",
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(5)
                .Column(0)
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3)
                ,

                // About
                new Label
                {
                    Text = "About",
                    TextColor = Colors.Black,
                    FontSize = 30
                }
                .Row(6)
                .Column(0)
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .ColumnSpan(3)

            }
        };
    }
}
