namespace FocusApp.Client.Helpers;

internal interface IAuthenticationService
{
    string Id { get; set; }
    string Email { get; set; }
    string AuthToken { get; set; }
    Shared.Models.User? CurrentUser { get; set; }
}

public class AuthenticationService : IAuthenticationService
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string AuthToken { get; set; } = "";
    public Shared.Models.User? CurrentUser { get; set; }
}
