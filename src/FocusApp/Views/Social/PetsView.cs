using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Resources.FontAwesomeIcons;

namespace FocusApp.Views.Social;

internal sealed class PetsView : ContentView
{
	public PetsView()
	{
		// Using grids
		Content = new Grid
		{
			// Define the length of the rows & columns
            // Rows: 80 for the top, 20 to act as padding, Stars for even spacing, and 140 for bottom padding
            // Columns: Two even columns should be enough
			RowDefinitions = Rows.Define(80, 20, Star, Star, Star, 140),
			ColumnDefinitions = Columns.Define(Star, Star),
			Children = {

				// Header
				new Label
				{
					Text = "Pets",
					TextColor = Colors.Black,
					FontSize = 40
				}
				.Row(0)
				.Column(0)
				.ColumnSpan(3)
				.CenterVertical()
				.Paddings(top : 5, bottom: 5, left: 75, right: 5),


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
				.Column(0)
                .Paddings(top : 5, bottom: 5, left: 10, right: 5)
				// When clicked, go to social view
				.Invoke(b => b.Clicked += (sender, e) => {Content = new MainView(); }),

				// Header & Content Divider
				new BoxView
				{
					Color = Colors.Black,
					WidthRequest = 400,
					HeightRequest = 2
				}
				.Bottom()
				.Row(0)
				.Column(0)
				.ColumnSpan(int.MaxValue),

				// Image Background 1
				new BoxView
				{
					Color = Colors.DarkGray,
					WidthRequest = 160,
					HeightRequest = 160,
					CornerRadius = 30
				}
				.Row(2)
				.Column(0)
				.ColumnSpan(0)
				.Center(),

				// Image Foreground 1
				new BoxView
				{
					Color = Colors.LightGrey,
					WidthRequest = 140,
					HeightRequest = 140,
					CornerRadius = 30
				}
				.Row(2)
				.Column(0)
				.Center(),

                // Pet Image 1
                new Image
                {
                    Source = "pet_greg.png",
                    WidthRequest = 120,
                    HeightRequest = 120
                }
                .Row(2)
                .Column(0)
                .Center(),
				

				// Image Background 2
				new BoxView
                {
                    Color = Colors.DarkGray,
                    WidthRequest = 160,
                    HeightRequest = 160,
                    CornerRadius = 30
                }
                .Row(2)
                .Column(1)
                .ColumnSpan(2)
                .Center(),

				// Image Foreground 2
				new BoxView
                {
                    Color = Colors.LightGrey,
                    WidthRequest = 140,
                    HeightRequest = 140,
                    CornerRadius = 30
                }
                .Row(2)
                .Column(1)
                .Center(),

                // Pet Image 2
                new Image
                {
                    Source = "pet_beans.png",
                    WidthRequest = 120,
                    HeightRequest = 120
                }
                .Row(2)
                .Column(1)
                .Center(),

				// Image Background 3
				new BoxView
                {
                    Color = Colors.DarkGray,
                    WidthRequest = 160,
                    HeightRequest = 160,
                    CornerRadius = 30
                }
                .Row(3)
                .Column(0)
                .ColumnSpan(0)
                .Center(),

				// Image Foreground 3
				new BoxView
                {
                    Color = Colors.LightGrey,
                    WidthRequest = 140,
                    HeightRequest = 140,
                    CornerRadius = 30
                }
                .Row(3)
                .Column(0)
                .Center(),

                // Pet Image 3
                new Image
                {
                    Source = "pet_bob.png",
                    WidthRequest = 120,
                    HeightRequest = 120
                }
                .Row(3)
                .Column(0)
                .Center(),

				// Image Background 4
				new BoxView
                {
                    Color = Colors.DarkGray,
                    WidthRequest = 160,
                    HeightRequest = 160,
                    CornerRadius = 30
                }
                .Row(3)
                .Column(1)
                .ColumnSpan(0)
                .Center(),

				// Image Foreground 4
				new BoxView
                {
                    Color = Colors.LightGrey,
                    WidthRequest = 140,
                    HeightRequest = 140,
                    CornerRadius = 30
                }
                .Row(3)
                .Column(1)
                .Center(),

                // Pet Image 4
                new Image
                {
                    Source = "pet_danole.png",
                    WidthRequest = 120,
                    HeightRequest = 120
                }
                .Row(3)
                .Column(1)
                .Center(),

				// Image Background 5
				new BoxView
                {
                    Color = Colors.DarkGray,
                    WidthRequest = 160,
                    HeightRequest = 160,
                    CornerRadius = 30
                }
                .Row(4)
                .Column(0)
                .ColumnSpan(0)
                .Center(),

				// Image Foreground 5
				new BoxView
                {
                    Color = Colors.LightGrey,
                    WidthRequest = 140,
                    HeightRequest = 140,
                    CornerRadius = 30
                }
                .Row(4)
                .Column(0)
                .Center(),

                // Pet Image 5
                new Image
                {
                    Source = "pet_franklin.png",
                    WidthRequest = 120,
                    HeightRequest = 120
                }
                .Row(4)
                .Column(0)
                .Center(),

				// Image Background 6
				new BoxView
                {
                    Color = Colors.DarkGray,
                    WidthRequest = 160,
                    HeightRequest = 160,
                    CornerRadius = 30
                }
                .Row(4)
                .Column(1)
                .ColumnSpan(0)
                .Center(),

				// Image Foreground 6
				new BoxView
                {
                    Color = Colors.LightGrey,
                    WidthRequest = 140,
                    HeightRequest = 140,
                    CornerRadius = 30
                }
                .Row(4)
                .Column(1)
                .Center(),

                // Pet Image 6
                new Image
                {
                    Source = "pet_wurmy.png",
                    WidthRequest = 120,
                    HeightRequest = 120
                }
                .Row(4)
                .Column(1)
                .Center(),
            }
		};
	}
}
