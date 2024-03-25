using System.Security.Claims;
using Auth0.OidcClient;
using FocusApp.Client.Clients;
using FocusApp.Shared.Data;
using FocusCore.Queries.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FocusApp.Client.Methods.User
{
    public class GetUserLogin
    {
        public class Query : IRequest<Result> { }

        public class Result
        { 
            public string? AuthToken { get; set; }
            public FocusApp.Shared.Models.User? CurrentUser { get; set; }
            public bool IsSuccessful { get; set; }
            public string? ErrorDescription { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly Auth0Client _auth0Client;
            IAPIClient _client;
            FocusAppContext _localContext;
            ILogger<Handler> _logger;
            public Handler(Auth0Client auth0Client, IAPIClient client, FocusAppContext localContext, ILogger<Handler> logger)
            {
                _auth0Client = auth0Client;
                _client = client;
                _localContext = localContext;
                _logger = logger;
            }

            public async Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                var loginResult = await _auth0Client.LoginAsync();

                if (!loginResult.IsError)
                {
                    var userIdentity = loginResult.User.Identity as ClaimsIdentity;
                    if (userIdentity != null)
                    {
                        IEnumerable<Claim> claims = userIdentity.Claims;

                        string auth0UserId = claims.First(c => c.Type == "sub").Value;
                        string userEmail = claims.First(c => c.Type == "email").Value;
                        string userName = claims.First(c => c.Type == "name").Value;

                        try
                        {
                            // Fetch user data from the server
                            Shared.Models.User user = await _client.GetUserByAuth0Id(new GetUserQuery
                            {
                                Auth0Id = auth0UserId,
                                Email = userEmail,
                                UserName = userName
                            });

                            // Add user to the local database if the user doesn't exist in the local database
                            if (!_localContext.Users.Any(u => u.Id == user.Id))
                            {
                                _localContext.Users.Add(user);
                                await _localContext.SaveChangesAsync();
                            }

                            return new Result
                            {
                                AuthToken = loginResult.AccessToken,
                                CurrentUser = user,
                                IsSuccessful = true,
                                ErrorDescription = null
                            };
                        }
                        catch (Exception ex)
                        {
                            _logger.Log(LogLevel.Error, "Error fetching user from server. Exception: " + ex.Message);
                        }
                    }
                }

                return new Result
                {
                    AuthToken = null,
                    CurrentUser = null,
                    IsSuccessful = false,
                    ErrorDescription = loginResult.ErrorDescription
                };
            }
        }
    }
}
