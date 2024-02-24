using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoucsApp.Client.Models;

namespace FocusApp.Client.Models;
public class AuthenticationHandler
{
    public Command LoginCommand
            => new Command(async () =>
            {
                var authUrl = $"https://accounts.google.com/o/oauth2/auth?response_type=code" +
                $"&redirect_uri=com.ZenPxl.FocusFriends://" +
                $"&client_id=271211607349-rrv5m9p48uv3edogcpdqkr9r6ql1650f.apps.googleusercontent.com" +
                $"&scope=https://www.googleapis.com/auth/userinfo.email" +
                $"&include_granted_scopes=true" +
                $"&state=state_parameter_passthrough_value";


                var callbackUrl = "com.ZenPxl.FocusFriends://";

                try
                {
                    var response = await WebAuthenticator.AuthenticateAsync(new WebAuthenticatorOptions()
                    {
                        Url = new Uri(authUrl),
                        CallbackUrl = new Uri(callbackUrl)
                    });

                    var codeToken = response.Properties["code"];

                    var parameters = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string,string>("grant_type","authorization_code"),
                    new KeyValuePair<string,string>("client_id","271211607349-rrv5m9p48uv3edogcpdqkr9r6ql1650f.apps.googleusercontent.com"),
                    new KeyValuePair<string,string>("redirect_uri",callbackUrl),
                    new KeyValuePair<string,string>("code",codeToken),
                });


                    HttpClient client = new HttpClient();
                    var accessTokenResponse = await client.PostAsync("https://oauth2.googleapis.com/token", parameters);

                    LoginResponse loginResponse;

                    if (accessTokenResponse.IsSuccessStatusCode)
                    {
                        var data = await accessTokenResponse.Content.ReadAsStringAsync();

                        loginResponse = JsonConvert.DeserializeObject<LoginResponse>(data);
                    }
                }
                catch (TaskCanceledException e)
                {
                    // Use stopped auth
                    Console.WriteLine(e.Message);
                    Console.WriteLine("It Done Broke");
                }


            });

}
