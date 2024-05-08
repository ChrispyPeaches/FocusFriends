using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Client.Methods.User
{
    internal class GetPersistentUserLogin
    {
        public class Query : IRequest<Result> { }

        public class Result
        { 
            public bool IsSuccessful { get; set; }
            public string Message { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            readonly FocusAppContext _localContext;
            UserManager _userManager;
            IAuthenticationService _authenticationService;
            public Handler(FocusAppContext localContext, UserManager userManager, IAuthenticationService authenticationService) 
            {
                _localContext = localContext;
                _userManager = userManager;
                _authenticationService = authenticationService;
            }

            public async Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                // Fetch and validate identity token from secure storage if the user has logged in previously
                var userPrincipal = await _userManager.GetAuthenticatedUser();

                if (userPrincipal != null)
                {
                    // Extract auth 0 id from claims, and use to fetch currently logged in user from local database
                    string auth0UserId = userPrincipal.Claims.First(c => c.Type == "sub").Value;

                    Shared.Models.User? user = await _localContext.Users
                        .Include(u => u.SelectedBadge)
                        .Include(u => u.SelectedDecor)
                        .Include(u => u.SelectedIsland)
                        .Include(u => u.SelectedPet)
                        .SingleOrDefaultAsync(u => u.Auth0Id == auth0UserId);

                    if (user == null)
                        throw new InvalidOperationException("User not found in the local database.");

                    // Set session variables
                    _authenticationService.PopulateWithUserData(user);

                    return new Result
                    {
                        IsSuccessful = true,
                        Message = "User successfully logged in via token in secure storage."
                    };
                }

                return new Result 
                {
                    IsSuccessful = false,
                    Message = "User was not found in database, or the user had no identity token in secure storage. Please manually log in as the user."
                };
            }
        }
    }
}
