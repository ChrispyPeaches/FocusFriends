using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Resources.FontAwesomeIcons;
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
                Text = "Friend " + i,
                ImageSource = new FileImageSource
                {
                    // Add logic that gets profile picture from friend user data
                    File = "dotnet_bot.png"
                },
                BindingContext = this
            });
        };

        DataTemplate dataTemplate = new DataTemplate(typeof(ImageCell));
        dataTemplate.SetBinding(ImageCell.TextProperty, "Text");
        dataTemplate.SetBinding(ImageCell.ImageSourceProperty, "ImageSource");

        Content = new Grid
        {
            // Define rows and columns (Star means that row/column will take up the remaining space)
            RowDefinitions = Rows.Define(80, Star, Star, Star),
            ColumnDefinitions = Columns.Define(Star, Star),
            BackgroundColor = Colors.LightGreen,
            Opacity = 0.9,

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
                new ListView
                {
                    Header = "Focusing",
                    ItemsSource = focusingFriends,
                    ItemTemplate = dataTemplate
                }
                .Row(1)
                .Column(0)
                .ColumnSpan(2),

                new Button
                {
                    Text = SolidIcons.Ankh,
                    TextColor = Colors.Black,
                    FontFamily = nameof(SolidIcons),
                    FontSize = 40,
                    BackgroundColor = Colors.Transparent
                }
                .Row(2)
                .Column(0)
                .Invoke(b => b.Clicked += (sender, e) => { Content = new AccountView(); }),

                new Button
                {
                    Text = SolidIcons.BanSmoking,
                    TextColor = Colors.Black,
                    FontFamily = nameof(SolidIcons),
                    FontSize = 40,
                    BackgroundColor = Colors.Transparent
                }
                .Row(2)
                .Column(1)
                .Invoke(b => b.Clicked += (sender, e) => { Content = new BadgeView(); }),

                new Button
                {
                    Text = SolidIcons.BreadSlice,
                    TextColor = Colors.Black,
                    FontFamily = nameof(SolidIcons),
                    FontSize = 40,
                    BackgroundColor = Colors.Transparent
                }
                .Row(3)
                .Column(0)
                .Invoke(b => b.Clicked += (sender, e) => { Content = new PetsView(); }),
            }
        };
    }
}