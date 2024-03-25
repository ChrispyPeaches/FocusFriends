using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Maui.Controls.Shapes;
using SimpleToolkit.SimpleShell.Extensions;

namespace FocusApp.Client.Views.Social
{
    internal class LeaderboardsPage : BasePage
    {
        enum PageRow { PageHeader, LeaderboardSelectors, TopThreeFriendsDisplay, RemainingFriendsDisplay, BottomWhiteSpace }
        enum PageColumn { DailyLeadboardButton, WeeklyLeaderboardButton }

        enum TopThreeRow { TopProfilePicture, GoldPillar, SilverPillar, BronzePillar }
        enum TopThreeColumn { Left, Center, Right }
        public LeaderboardsPage()
        {
            Grid TopThreeFriendsGrid = GetTopThreeFriendsGrid();

            Content = new Grid
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                    (PageRow.PageHeader, GridRowsColumns.Stars(1)),
                    (PageRow.LeaderboardSelectors, GridRowsColumns.Stars(1)),
                    (PageRow.TopThreeFriendsDisplay, GridRowsColumns.Stars(4.5)),
                    (PageRow.RemainingFriendsDisplay, GridRowsColumns.Stars(3.5)),
                    (PageRow.BottomWhiteSpace, GridRowsColumns.Stars(1.25))
                    ),
                ColumnDefinitions = GridRowsColumns.Columns.Define(
                    (PageColumn.DailyLeadboardButton, GridRowsColumns.Stars(1)),
                    (PageColumn.WeeklyLeaderboardButton, GridRowsColumns.Stars(1))
                    ),
                BackgroundColor = AppStyles.Palette.OrchidPink,
                Children =
                { 
                    // Back Button
				    new Button
                    {
                        Text = SolidIcons.ChevronLeft,
                        TextColor = Colors.Black,
                        FontFamily = nameof(SolidIcons),
                        FontSize = 40,
                        BackgroundColor = Colors.Transparent
                    }
                    .Row(PageRow.PageHeader)
                    .Left()
                    .CenterVertical()
				    // When clicked, go to social view
				    .Invoke(button => button.Released += (sender, eventArgs) =>
                        BackButtonClicked(sender, eventArgs)),

                    // Page Header
                    new Label
                    { 
                        Text = "Leaderboards",
                        TextColor = Colors.Black,
                        FontSize = 30,
                        FontAttributes = FontAttributes.Bold
                    }
                    .Row(PageRow.PageHeader)
                    .Top()
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                    .CenterHorizontal()
                    .CenterVertical(),

                    // Horizontal Divider
                    new BoxView
                    {
                        Color = Color.Parse("Black"),
                        HeightRequest = 2,
                    }
                    .Row(PageRow.PageHeader)
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                    .Bottom(),

                    // Daily Leaderboards Button
                    new Button
                    { 
                        Text = "Daily",
                        Margin = 15
                    }
                    .Row(PageRow.LeaderboardSelectors)
                    .Column(PageColumn.DailyLeadboardButton),

                    // Weekly Leaderboards Button
                    new Button
                    {
                        Text = "Weekly",
                        Margin = 15
                    }
                    .Row(PageRow.LeaderboardSelectors)
                    .Column(PageColumn.WeeklyLeaderboardButton),

                    // Top Three Friends Leaderboard Display
                    TopThreeFriendsGrid
                    .Row(PageRow.TopThreeFriendsDisplay)
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length),

                    // Remaining Friends Leaderboard Display
                    new Grid 
                    {
                        BackgroundColor = AppStyles.Palette.DarkPeriwinkle
                    }
                    .Row(PageRow.RemainingFriendsDisplay)
                    .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                }
            };
        }

        // TODO: Make this dynamic
        private Grid GetTopThreeFriendsGrid()
        {
            // Top Three Friends Leaderboard Display
            Grid topThreeFriendsGrid = new Grid
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                    (TopThreeRow.TopProfilePicture, GridRowsColumns.Stars(1)),
                    (TopThreeRow.GoldPillar, GridRowsColumns.Stars(1)),
                    (TopThreeRow.SilverPillar, GridRowsColumns.Stars(1)),
                    (TopThreeRow.BronzePillar, GridRowsColumns.Stars(1))
                    ),
                ColumnDefinitions = GridRowsColumns.Columns.Define(
                    (TopThreeColumn.Left, GridRowsColumns.Stars(1)),
                    (TopThreeColumn.Center, GridRowsColumns.Stars(1)),
                    (TopThreeColumn.Right, GridRowsColumns.Stars(1))
                    ),
                Children =
                {
                    // Bronze Pillar
                    new Frame
                    {
                        BackgroundColor = Colors.RosyBrown,
                        Content = 
                            // Third Place Points
                            new Label
                            {
                                Text = "10"
                            }
                            .Row(TopThreeRow.BronzePillar)
                            .Column(TopThreeColumn.Left)
                            .CenterHorizontal()
                    }
                    .Row(TopThreeRow.BronzePillar)
                    .Column(TopThreeColumn.Left),

                    // Third Place Friend
                    new Image
                    {
                        Source = new FileImageSource
                        {
                            // Add logic that gets profile picture from user data
                            File = "dotnet_bot.png"
                        },
                    }
                    // Note: Center denotes where within the grid cell the image will be centered
                    //       Find a way to do this programatically
                    .Clip(new EllipseGeometry { Center = new Point(64, 35), RadiusX = 27, RadiusY = 27 })
                    .Row(TopThreeRow.SilverPillar)
                    .Column(TopThreeColumn.Left)
                    .CenterHorizontal()
                    .CenterVertical(),

                    // Silver Pillar
                    new Frame
                    {
                        BackgroundColor = Colors.Silver,
                        Content =
                            // Second Place Points
                            new Label
                            {
                                Text = "20"
                            }
                            .Row(TopThreeRow.SilverPillar)
                            .Column(TopThreeColumn.Right)
                            .CenterHorizontal()
                    }
                    .Row(TopThreeRow.SilverPillar)
                    .Column(TopThreeColumn.Right)
                    .RowSpan(2),

                    // Second Place Friend
                    new Image
                    {
                        Source = new FileImageSource
                        {
                            // Add logic that gets profile picture from user data
                            File = "dotnet_bot.png"
                        },
                    }
                    // Note: Center denotes where within the grid cell the image will be centered
                    //       Find a way to do this programatically
                    .Clip(new EllipseGeometry { Center = new Point(64, 35), RadiusX = 27, RadiusY = 27 })
                    .Row(TopThreeRow.GoldPillar)
                    .Column(TopThreeColumn.Right)
                    .CenterHorizontal()
                    .CenterVertical(),

                    // Gold Pillar
                    new Frame
                    {
                        BackgroundColor = Colors.Gold,
                        Content =                             
                            // First Place Points
                            new Label
                            {
                                Text = "30",
                                TextColor = Colors.Black
                            }
                            .Row(TopThreeRow.GoldPillar)
                            .Column(TopThreeColumn.Center)
                            .CenterHorizontal()
                    }
                    .Row(TopThreeRow.GoldPillar)
                    .Column(TopThreeColumn.Center)
                    .RowSpan(3),

                    // First Place Friend
                    new Image
                    {
                        Source = new FileImageSource
                        {
                            // Add logic that gets profile picture from user data
                            File = "dotnet_bot.png"
                        },
                    }
                    // Note: Center denotes where within the grid cell the image will be centered
                    //       Find a way to do this programatically
                    .Clip(new EllipseGeometry { Center = new Point(64, 35), RadiusX = 27, RadiusY = 27 })
                    .Row(TopThreeRow.TopProfilePicture)
                    .Column(TopThreeColumn.Center)
                    .CenterHorizontal()
                    .CenterVertical(),
                }
            };

            return topThreeFriendsGrid;
        }

        private async void BackButtonClicked(object sender, EventArgs e)
        {
            // Back navigation reverses animation so can keep right to left
            Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
            await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
        }
    }
}
