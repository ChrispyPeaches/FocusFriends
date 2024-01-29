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
			RowDefinitions = Rows.Define(80, Star, Star, Star, Star, Star, Star, Star, Star),
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
				.Column(1)
				.ColumnSpan(3)
				.CenterVertical()
				.Paddings(top : 5, bottom: 5, left: 5, right: 5),


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
                .Paddings(top : 5, bottom: 5, left: 5, right: 5)
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

				// Image 1
				new BoxView
				{
					Color = Colors.DarkGray,
					WidthRequest = 150,
					HeightRequest = 150,
					CornerRadius = 30,
					Shadow = new Shadow
					{
						Brush = Colors.Black,
						Opacity = 0.35f
					}
				}
				.Row(2)
				.Column(0)
				.ColumnSpan(0)
				.Center()
				.Bottom(),

				// Sub Image 1
				

				// Image 2
				new BoxView
                {
                    Color = Colors.DarkGray,
                    WidthRequest = 150,
                    HeightRequest = 150,
                    CornerRadius = 30
                }
                .Row(2)
                .Column(1)
                .ColumnSpan(2)
                .Center()
                .Bottom()
            }
		};
	}
}
