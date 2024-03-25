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
using SimpleToolkit.SimpleShell.Extensions;

namespace FocusApp.Client.Views.Social
{
    internal class LeaderboardsPage : BasePage
    {
        enum Row { PageHeader, LeaderboardSelectors, TopThreeFriendsDisplay, RemainingFriendsDisplay, BottomWhiteSpace }
        enum Column { DailyLeadboardButton, WeeklyLeaderboardButton }
        public LeaderboardsPage()
        {
            Content = new Grid
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                    (Row.PageHeader, GridRowsColumns.Stars(1)),
                    (Row.LeaderboardSelectors, GridRowsColumns.Stars(1)),
                    (Row.TopThreeFriendsDisplay, GridRowsColumns.Stars(4)),
                    (Row.RemainingFriendsDisplay, GridRowsColumns.Stars(4)),
                    (Row.BottomWhiteSpace, GridRowsColumns.Stars(1.25))
                    ),
                ColumnDefinitions = GridRowsColumns.Columns.Define(
                    (Column.DailyLeadboardButton, GridRowsColumns.Stars(1)),
                    (Column.WeeklyLeaderboardButton, GridRowsColumns.Stars(1))
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
                    .Row(Row.PageHeader)
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
                    .Row(Row.PageHeader)
                    .Top()
                    .ColumnSpan(typeof(Column).GetEnumNames().Length)
                    .CenterHorizontal()
                    .CenterVertical(),

                    // Horizontal Divider
                    new BoxView
                    {
                        Color = Color.Parse("Black"),
                        HeightRequest = 2,
                    }
                    .Row(Row.PageHeader)
                    .ColumnSpan(typeof(Column).GetEnumNames().Length)
                    .Bottom(),

                    // Daily Leaderboards Button
                    new Button
                    { 
                        Text = "Daily",
                        Margin = 15
                    }
                    .Row(Row.LeaderboardSelectors)
                    .Column(Column.DailyLeadboardButton),

                    // Weekly Leaderboards Button
                    new Button
                    {
                        Text = "Weekly",
                        Margin = 15
                    }
                    .Row(Row.LeaderboardSelectors)
                    .Column(Column.WeeklyLeaderboardButton),

                    // Top Three Friends Leaderboard Display
                    new Frame
                    { 
                        BorderColor = Colors.Black,
                    }
                    .Row(Row.TopThreeFriendsDisplay)
                    .ColumnSpan(typeof(Column).GetEnumNames().Length),

                    // Remaining Friends Leaderboard Display
                    new Frame 
                    { 
                        BorderColor = Colors.Black, 
                    }
                    .Row(Row.RemainingFriendsDisplay)
                    .ColumnSpan(typeof(Column).GetEnumNames().Length)
                }
            };
        }
        private async void BackButtonClicked(object sender, EventArgs e)
        {
            // Back navigation reverses animation so can keep right to left
            Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
            await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
        }
    }
}
