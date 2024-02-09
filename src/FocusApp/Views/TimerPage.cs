using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Clients;
using FocusApp.Dtos;
using FocusApp.Helpers;
using FocusApp.Resources;
using FocusApp.Resources.FontAwesomeIcons;
using FocusAppShared.Data;
using FocusCore.Queries.User;

namespace FocusApp.Views
{
    internal class TimerPage : BasePage
    {
        private ITimerService _timerService;
        private IDispatcherTimer? _timeStepperTimer;
        IAPIClient _client;

        enum Row { TopBar, TimerDisplay, Island, PetAndIsland, MiddleWhiteSpace, TimerButtons, BottomWhiteSpace }
        enum Column { LeftTimerButton, TimerAmount, RightTimerButton }
        public enum TimerButton { Up, Down }

        public TimerPage(IAPIClient client, ITimerService timerService)
        {
            _client = client;
            _timerService = timerService;

            Island islandPlaceholder = new Island()
            {
                Name = "Default",
                ImagePath = "island_zero.png"
            };

            Pet petPlaceholder = new Pet()
            {
                Name = "Cat",
                ImagePath = "pet_cat_zero.png",
                HeightRequest = 90
            };

            Content = new Grid
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                    (Row.TopBar, GridRowsColumns.Stars(1)),
                    (Row.TimerDisplay, GridRowsColumns.Stars(1)),
                    (Row.Island, GridRowsColumns.Stars(3)),
                    (Row.PetAndIsland, GridRowsColumns.Stars(1)),
                    (Row.MiddleWhiteSpace, GridRowsColumns.Stars(1)),
                    (Row.TimerButtons, GridRowsColumns.Stars(1)),
                    (Row.BottomWhiteSpace, GridRowsColumns.Stars(1))
                    ),
                ColumnDefinitions = GridRowsColumns.Columns.Define(
                    (Column.LeftTimerButton, GridRowsColumns.Stars(1)),
                    (Column.TimerAmount, GridRowsColumns.Stars(2)),
                    (Column.RightTimerButton, GridRowsColumns.Stars(1))
                    ),
                BackgroundColor = AppStyles.Palette.LightPeriwinkle,
                Children =
                {
                    // Setting Button
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
                    .Bind(IsVisibleProperty,
                            getter: (ITimerService th) => th.AreStepperButtonsVisible, source: _timerService)
                    .Invoke(button => button.Released += (sender, eventArgs) =>
                            SettingsButtonClicked(sender, eventArgs)),

                    // Time Left Display
                    new Label
                    {
                        BindingContext = _timerService,
                        FontSize = 70,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                    .Center()
                    .Row(Row.TimerDisplay)
                    .ColumnSpan(typeof(Column).GetEnumNames().Length)
                    .Bind(Label.TextProperty,
                            getter: static (ITimerService th) => th.TimerDisplay),

                    // Island
                    new Image
                    {
                        Source = islandPlaceholder.ImagePath,
                    }
                    .Row(Row.Island)
                    .RowSpan(3)
                    .ColumnSpan(typeof(Column).GetEnumNames().Length)
                    .Margins(left: 10, right: 10),

                    // Pet
                    new Image
                    {
                        Source = petPlaceholder.ImagePath,
                        MaximumHeightRequest = 200,
                        HeightRequest = petPlaceholder.HeightRequest
                    }
                    .Row(Row.PetAndIsland)
                    .Column(Column.TimerAmount)
                    .Margins(bottom: 60)
                    .Bottom()
                    .End(),

                    // Increase Time Button
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
                    .Column(Column.LeftTimerButton)
                    .Bind(IsVisibleProperty,
                            getter: (ITimerService th) => th.AreStepperButtonsVisible, source: _timerService)
                    .Invoke(button => button.Clicked += (sender, eventArgs) => 
                            onTimeStepperButtonClick(TimerButton.Up))
                    .Invoke(button => button.Pressed += (sender, eventArgs) =>
                            onTimeStepperButtonPressed(TimerButton.Up))
                    .Invoke(button => button.Released += (sender, eventArgs) =>
                            onTimeStepperButtonReleased()),

                    // Toggle Timer Button
                    new Button
                    {
                        BindingContext = _timerService,
                        TextColor = Colors.Black,
                        CornerRadius = 20,
                    }
                    .Font(size: 20).Margins(left: 10, right: 10)
                    .CenterVertical()
                    .Row(Row.TimerButtons)
                    .Column(Column.TimerAmount)
                    .Bind(Button.TextProperty,
                            getter: static (ITimerService th) => th.ToggleTimerButtonText)
                    .Bind(BackgroundColorProperty,
                            getter: static (ITimerService th) => th.ToggleTimerButtonBackgroudColor)
                    .Invoke(button => button.Clicked += (sender, eventArgs) =>
                            _timerService.ToggleTimer.Invoke()),

                    // Decrease Time Button
                    new Button
                    {
                        BindingContext = _timerService,
                        Text = SolidIcons.ChevronDown,
                        BackgroundColor = Colors.Transparent,
                        TextColor = Colors.Black
                    }
                    .Font(family: nameof(SolidIcons), size: 40)
                    .Start()
                    .CenterVertical()
                    .Row(Row.TimerButtons)
                    .Column(Column.RightTimerButton)
                    .Bind(IsVisibleProperty, 
                            getter: (ITimerService th) => th.AreStepperButtonsVisible, source: _timerService )
                    .Invoke(button => button.Clicked += (sender, eventArgs) =>
                            onTimeStepperButtonClick(TimerButton.Down))
                    .Invoke(button => button.Pressed += (sender, eventArgs) =>
                            onTimeStepperButtonPressed(TimerButton.Down))
                    .Invoke(button => button.Released += (sender, eventArgs) =>
                            onTimeStepperButtonReleased()),
                }
            };
        }

        /// <summary>
        /// Increment or decrement the timer duration.
        /// </summary>
        public void onTimeStepperButtonClick(TimerButton clickedButton)
        {
            int _stepRate = (int)TimeSpan.FromMinutes(1).TotalSeconds;

            _timerService.TimeLeft = clickedButton switch
            {
                TimerButton.Up => _timerService.TimeLeft + _stepRate,
                TimerButton.Down => (_timerService.TimeLeft > _stepRate) ?
                                                    _timerService.TimeLeft - _stepRate
                                                    : _stepRate,
                _ => 0
            };
        }

        /// <summary>
        /// Start the time duration stepper timer while the user holds the button.
        /// </summary>
        public void onTimeStepperButtonPressed(TimerButton clickedButton)
        {
            _timeStepperTimer = Application.Current!.Dispatcher.CreateTimer();
            _timeStepperTimer.Interval = TimeSpan.FromMilliseconds(200);
            _timeStepperTimer.Tick += (sender, e) => onTimeStepperButtonClick(clickedButton);
            _timeStepperTimer.Start();
        }

        /// <summary>
        /// Stop the time duration stepper timer.
        /// </summary>
        public void onTimeStepperButtonReleased()
        {
            if (_timeStepperTimer is not null)
            {
                _timeStepperTimer.Stop();
                _timeStepperTimer = null;
            }
        }

        private async void SettingsButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///" + nameof(SettingsPage));
        }

        protected override async void OnAppearing()
        {
            var user = await _client.GetUser(new GetUserQuery { Id = Guid.NewGuid() } );
            base.OnAppearing();
        }
    }
}