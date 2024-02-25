using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusApp.Client.Helpers;

internal interface IAuthenticationService
{
    string Id { get; set; }
    string Email { get; set; }
    string AuthToken { get; set; }
}

public class AuthenticationService : IAuthenticationService
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string AuthToken { get; set; } = "";
}
