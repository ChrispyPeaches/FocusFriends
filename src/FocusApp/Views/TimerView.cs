namespace FocusApp.Views
{
    internal class TimerView : ContentView
    {
        public TimerView()
        {
            Content = new Grid
            {
                BackgroundColor = Colors.PeachPuff,
                Children =
                {
                    new Label
                    {
                        Text = "Timer",
                        FontSize = 30,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                }
            };
        }
    }
}
