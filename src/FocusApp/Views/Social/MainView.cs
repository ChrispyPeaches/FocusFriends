using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace FocusApp.Views.Social;

public class MainView : ContentView
{
	public MainView()
	{
        // Add logic to fetch focused friends
        List<ImageCell> focusingFriends = new List<ImageCell>();
        for (int i = 0; i < 5; i++)
{
            focusingFriends.Add(new ImageCell
    {
                Text = "Friend" + i,
                ImageSource = new FileImageSource
        {
                        // Add logic that gets profile picture from friend user data
                        File = "dotnet_bot.png"
                    },
                BindingContext = this
            });
        };

            Content = new Grid
            {
            // Define rows and columns (Star means that row/column will take up the remaining space)
            RowDefinitions = Rows.Define(80, Star),
            ColumnDefinitions = Columns.Define(Star, Star),

                Children =
                {
                // Header
                    new Label
                    {
                    Text = "Friends",
                    FontSize = 40
                }
                .Row(0)
                .Column(0)
                .Padding(15, 15),

                // Horizontal Divider
                new BoxView
                {
                    Color = Color.Parse("Black"),
                    WidthRequest = 400,
                    HeightRequest = 2
                }
                .Bottom()
                .Row(0)
                .Column(0)
                .ColumnSpan(2),

                // Profile Picture
                new Image
                {
                    Source = new FileImageSource
                    {
                        // Add logic that gets profile picture from user data
                        File = "dotnet_bot.png"
                    },
                    WidthRequest = 90,
                    HeightRequest = 90
                }
                .Top()
                .Right()
                .Column(1)
                .Clip(new EllipseGeometry { Center = new Point(43, 45), RadiusX = 27, RadiusY = 27 }),

                // Friends List
                new ListView()
                {
                    Header = "Focusing",
                    ItemsSource = focusingFriends
                    //ItemTemplate = new DataTemplate(typeof(ImageCell)),
                    //BindingContext = ImageCell.BindingContextProperty
                    }
                .Row(1)
                .Column(0)
                .ColumnSpan(2)
                }
            };
        }
    }
}