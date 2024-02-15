using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Clients;
using FocusCore.Queries.User;
using FocusApp.Client.Views;

namespace FocusApp.Client.Views;

internal class LoginPage : BasePage
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
                    TextColor = Colors.Black,
                    CornerRadius = 20,
                    BackgroundColor = AppStyles.Palette.Celeste
                }
                .Row(0)
                .Top()
                .Right()
                .Font(size: 15).Margins(top: 10, bottom: 10, left: 10, right: 10)
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    SkipButtonClicked(sender, eventArgs)),

				// Login Text
				new Label
                {
                    Text = "Login",
                    TextColor = Colors.Black,
                    FontSize = 40
                }
                .Row(2)
                .Center(),

				// Google sign in button
				new ImageButton
                {
                    Source = "g_sign_in"
                }
                .Row(3)
                .Center()
				// Logic when clicked
				.Invoke(button => button.Released += (sender, eventArgs) =>
                    GoogleSignInClicked(sender, eventArgs)),
            }
        };
    }

    private async void SkipButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///" + nameof(TimerPage));
    }

    private void GoogleSignInClicked(object sender, EventArgs e)
    {
        Console.WriteLine("Clicked");
    }

    protected override async void OnAppearing()
    {
        var user = await _client.GetUser(new GetUserQuery { Id = Guid.NewGuid() });
        base.OnAppearing();
    }
}

