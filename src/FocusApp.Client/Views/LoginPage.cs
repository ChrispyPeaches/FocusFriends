using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources;
using FocusApp.Client.Clients;
using FocusCore.Queries.User;
using Auth0.OidcClient;
using FocusApp.Client.Helpers;
using System.Security.Claims;
using FocusCore.Models;

namespace FocusApp.Client.Views;

internal class LoginPage : BasePage
{
    IAPIClient _client;
    private readonly Auth0Client auth0Client;
    IAuthenticationService _authenticationService;

    public LoginPage(IAPIClient client, Auth0Client authClient, IAuthenticationService authenticationService)
    {
        _client = client;
        auth0Client = authClient;
        _authenticationService = authenticationService;

        var pets = new List<string> { "pet_beans.png", "pet_bob.png", "pet_danole.png", "pet_franklin.png", "pet_greg.png", "pet_wurmy.png" };
        var rnd = new Random();

        Content = new Grid
        {
            RowDefinitions = Rows.Define(200, 100, 100, 80, 80, Star, 50),
            ColumnDefinitions = Columns.Define(Star),
            BackgroundColor = AppStyles.Palette.LightPeriwinkle,

            Children =
            {
                // Pet Image
                new Image
                {
                    Source = pets[rnd.Next(0, pets.Count)],
                    WidthRequest = 150,
                    HeightRequest = 150
                }
                .Row(0)
                .Center()
                .Bottom(),

                // Label
                new Label
                {
                    Text = "Focus Friends",
                    TextColor = Colors.Black,
                }
                .Row(1)
                .Center()
                .Font(size: 40).Margins(top: 10, bottom: 10, left: 10, right: 10),

                // Login/Signup Button
				new Button
                {
                    Text = "Login/Signup",
                    TextColor = Colors.Black,
                    CornerRadius = 20,
                    BackgroundColor = AppStyles.Palette.Celeste
                }
                .Row(3)
                .CenterHorizontal()
                .Font(size: 25).Margins(top: 10, bottom: 10, left: 10, right: 10)
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    OnLoginClicked(sender, eventArgs)),

				// Skip Login Button
				new Button
                {
                    Text = "Continue Without Login",
                    TextColor = Colors.Black,
                    CornerRadius = 20,
                    BackgroundColor = AppStyles.Palette.Celeste
                }
                .Row(4)
                .CenterHorizontal()
                .Font(size: 25).Margins(top: 10, bottom: 10, left: 10, right: 10)
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    SkipButtonClicked(sender, eventArgs)),

                // Logo 
                new Image
                {
                    Source = "logo.png",
                    WidthRequest = 75,
                    HeightRequest = 75,
                }
                .Row(5)
                .Center()
            }
        };
    }

    private async void SkipButtonClicked(object sender, EventArgs e)
    {
        // If user skips login, set current user to null
        _authenticationService.CurrentUser = null;
        await Shell.Current.GoToAsync("///" + nameof(TimerPage));
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var loginResult = await auth0Client.LoginAsync();

        if (!loginResult.IsError)
        {
            _authenticationService.AuthToken = loginResult.AccessToken;
            Console.WriteLine("Login Page: " + _authenticationService.AuthToken);

            var userIdentity = loginResult.User.Identity as ClaimsIdentity;
            if (userIdentity != null)
            {
                IEnumerable<Claim> claims = userIdentity.Claims;

                string auth0UserId = claims.First(c => c.Type == "sub").Value;
                string userEmail = claims.First(c => c.Type == "email").Value;
                string userName = claims.First(c => c.Type == "name").Value;

                BaseUser user = await _client.GetUserByAuth0Id(new GetUserQuery
                {
                    Auth0Id = auth0UserId,
                    Email = userEmail,
                    UserName = userName
                });

                _authenticationService.CurrentUser = user;
            }
            
            await Shell.Current.GoToAsync($"///" + nameof(TimerPage));
        }
        else
        {
            await DisplayAlert("Error", loginResult.ErrorDescription, "OK");
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }
}

