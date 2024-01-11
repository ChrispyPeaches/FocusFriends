using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace FocusApp.Views;

internal sealed class SettingsView : ContentView
{
    public SettingsView()
    {
        // Using grids
        Content = new Grid
        {
            // Define the lenth of the rows & columns
            RowDefinitions = Rows.Define(80, Star),
            ColumnDefinitions = Columns.Define(Star, Star),

            Children =
            {
                // Header
                new Label
                {
                    Text = "Settings",
                    FontSize = 40
                }
                .Row(0)
                .Column(5)
                .Padding(15),

                // Button
                new ImageButton
                {
                     Source = "back_arrow.png"
                }
                .Left()
                .Center()
                .Column(0)
                // Logic for the button press
                .Invoke(b => b.Clicked += (sender, e) => {Console.WriteLine("Clicked"); },

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
                .ColumnSpan(2),


            }
        };
    }
}
