namespace FocusApp.Pages.Social;

internal sealed class MainPage : BasePage
{
	public MainPage()
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
                    Text = "Social Page!"
                }
            }
        };
    }
}