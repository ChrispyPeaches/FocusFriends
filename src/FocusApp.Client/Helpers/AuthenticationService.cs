﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocusCore.Models;

namespace FocusApp.Client.Helpers;

internal interface IAuthenticationService
{
    string Id { get; set; }
    string Email { get; set; }
    string AuthToken { get; set; }
    BaseUser CurrentUser { get; set; }
}

public class AuthenticationService : IAuthenticationService
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string AuthToken { get; set; } = "";
    public BaseUser CurrentUser { get; set; }
}
