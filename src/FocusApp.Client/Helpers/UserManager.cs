using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityModel.Client;
using IdentityModel.OidcClient;

namespace FocusApp.Client.Helpers;
internal interface IUserManager
{
    public Task<ClaimsPrincipal> GetAuthenticatedUser();
}

public class UserManager : IUserManager
{
    private string _domain = "";
    private string _clientId = "";

    public UserManager(string domain, string clientId)
    { 
        _domain = domain;
        _clientId = clientId;
    }

    public async Task<string?> GetIdToken()
    {
        return await SecureStorage.Default.GetAsync("id_token");
    }



    public async Task<ClaimsPrincipal> GetAuthenticatedUser()
    { 
        ClaimsPrincipal user = null;
        var idToken = await SecureStorage.Default.GetAsync("id_token");

        if (idToken != null)
        { 
            var doc = await new HttpClient().GetDiscoveryDocumentAsync($"https://{_domain}");
            var validator = new JwtHandlerIdentityTokenValidator();

            var options = new OidcClientOptions
            {
                ClientId = _clientId,
                ProviderInformation = new ProviderInformation
                { 
                    IssuerName = doc.Issuer,
                    KeySet = doc.KeySet
                }
            };

            var validationResult = await validator.ValidateAsync(idToken, options);

            if (!validationResult.IsError)
                user = validationResult.User;
        }

        return user;
    }
}

