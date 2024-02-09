using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Resources;
using FocusApp.Resources.FontAwesomeIcons;
using FocusApp.Clients;
using FocusCore.Queries.User;

namespace FocusApp.Views;

internal class LoginPage : ContentPage
{
	IAPIClient _client;
	public LoginPage(IAPIClient client)
	{
		_client = client;

		Content = new Grid
		{
			RowDefinitions = Rows.Define(60, 120, 80, 80, 80, 80, Star),
			ColumnDefinitions = Columns.Define(Star),
			BackgroundColor = AppStyles.Palette.LightPeriwinkle,

			Children =
			{
				// Skip Login Button
				new Button
				{
					Text = "Skip",
					MaximumHeightRequest = 50,
					MaximumWidthRequest = 100
				}
				.Row(0)
				.Right()
                .Paddings(top: 5, bottom: 5, left: 5, right: 10)
                .CenterVertical(),

				// Login Text
				new Label
				{
					Text = "Login With Google",
					TextColor = Colors.Black,
					FontSize = 40
				}
				.Row(2)
				.Center()
			}
		};
	}

	private async void SkipButtonClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("///" + nameof(TimerPage));
	}
	
	protected override async void OnAppearing()
	{
		var user = await _client.GetUser(new GetUserQuery { Id = Guid.NewGuid() });
		base.OnAppearing();
	}
}

