using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using Microsoft.Maui.Controls.Shapes;

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
                    Text = "Friends",
                    FontSize = 40
                }
                .Top()
                .Left()
                .Padding(15, 15),

                new Image
                {
                    Source = new FileImageSource
                    {
                        File = "dotnet_bot.png",
                    },
                    WidthRequest = 90,
                    HeightRequest =90
                }
                .Top()
                .Right()
                .Clip(new EllipseGeometry { Center = new Point(43, 45), RadiusX = 27, RadiusY = 27 })
    }
        };
    }
}