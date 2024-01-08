namespace FocusApp.Pages.Social;

internal sealed class MainPage : BasePage
{
	public MainPage()
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
                    Text = "Social",
                    FontSize = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            }
        };
    }
}