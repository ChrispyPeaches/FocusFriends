namespace FocusApp.Pages;

internal sealed class TimerPage : BasePage
{
	public TimerPage()
	{
	}

	public override void Build()
	{
        Content = new StackLayout
		{
            Children =
			{
                new Label
				{
                    Text = "Timer Page!"
                }
            }
        };
    }
}