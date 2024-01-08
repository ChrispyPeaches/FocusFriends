namespace FocusApp.Pages;

internal sealed class TimerPage : BasePage
{
	public TimerPage()
	{
	}

	public override void Build()
	{
        Content = new Grid
        {
            Children =
            {
                new Label
                {
                    Text = "Timer",
                    FontSize = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            }
        };
    }
}