namespace FocusApp.Pages.Shop;

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
                    Text = "Shop",
                    FontSize = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            }
        };
    }
}