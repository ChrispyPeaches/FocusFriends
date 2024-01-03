namespace FocusApp.Pages.Shop;

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
                    Text = "Shop Page!"
                }
            }
        };
    }
}