using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Helpers;
using FocusApp.Resources.FontAwesomeIcons;

namespace FocusApp.Views
{
    internal class TimerView : ContentView
    {
        private TimerHelper _timerHelper;

        enum Row { TopBar, TimerDisplay, Island, TimerButtons, BottomWhiteSpace }
        enum Column { LeftTimerButton, TimerAmount, RightTimerButton }

        public TimerView()
        {
            _timerHelper = new TimerHelper();

            Content = new Grid
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                    ( Row.TopBar, GridRowsColumns.Stars(1)           ),
                    ( Row.TimerDisplay, GridRowsColumns.Stars(2)     ),
                    ( Row.Island, GridRowsColumns.Stars(3)           ),
                    ( Row.TimerButtons, GridRowsColumns.Stars(1)     ),
                    ( Row.BottomWhiteSpace, GridRowsColumns.Stars(1) )
                    ),
                ColumnDefinitions = GridRowsColumns.Columns.Define(
                    (Column.LeftTimerButton, GridRowsColumns.Stars(1)),
                    (Column.TimerAmount, GridRowsColumns.Stars(2)),
                    (Column.RightTimerButton, GridRowsColumns.Stars(1))
                    ),
                BackgroundColor = Color.FromArgb("BBD0FF"),
                Children =
                {
                    new Button
                    {
                        Text = SolidIcons.Gears,
                        TextColor = Colors.Black,
                        FontFamily = nameof(SolidIcons),
                        FontSize = 40,
                        BackgroundColor = Colors.Transparent
                    }
                    .Row(Row.TopBar)
                    .Top()
                    .Left()
                    .Invoke(b => b.Clicked += (sender, e) => { Content = new SettingsView(); }),

                    new Button
                    {
                        Text = SolidIcons.ChevronUp,
                        BackgroundColor = Colors.Transparent,
                        TextColor = Colors.Black,
                        FontSize = 30,
                    }
                    .Font(family: nameof(SolidIcons), size: 40)
                    .End()
                    .CenterVertical()
                    .Row(Row.TimerButtons)
                    .Column(Column.LeftTimerButton),

                    new Label
                    {
                        BindingContext = _timerHelper,
                        FontSize = 30,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                    .Row(Row.TimerDisplay)
                    .Column(Column.TimerAmount)
                    .Bind(Label.TextProperty, getter: static (TimerHelper th) => th.TimerDisplay),

                    new Button
                    {
                        Text = SolidIcons.ChevronDown,
                        BackgroundColor = Colors.Transparent,
                        TextColor = Colors.Black,
                    }
                    .Font(family: nameof(SolidIcons), size: 40)
                    .Start()
                    .CenterVertical()
                    .Row(Row.TimerButtons)
                    .Column(Column.RightTimerButton),
                }
            };
        }
    }
}
