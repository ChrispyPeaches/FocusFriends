using System.Linq;
using System.Net;
using System.Security.Claims;
using Auth0.OidcClient;
using FocusApp.Client.Clients;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using FocusCore.Queries.User;
using FocusCore.Responses.User;
using IdentityModel.OidcClient;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Refit;

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
                LoginResult? loginResult = await MainThread
                    .InvokeOnMainThreadAsync(() => _auth0Client.LoginAsync(cancellationToken: cancellationToken));

                Shared.Models.User? user = null;

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
                            ApiResponse<GetUserResponse>? response = await _client.GetUserByAuth0Id(
                                new GetUserQuery
                                {
                                    Auth0Id = auth0UserId
                                },
                                cancellationToken);

                            switch (response.StatusCode)
                            {
                                case HttpStatusCode.NotFound:
                                    user = await CreateUser(auth0UserId, userEmail, userName, cancellationToken);
                                    break;
                                case HttpStatusCode.InternalServerError:
                                    throw new Exception("Error fetching user from server.");
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
                            _logger.LogError(ex, "Error getting or creating user.");
                            return new Result
                            {
                                IsSuccessful = false,
                                ErrorDescription = ex.Message
                            };
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

            private async Task<Shared.Models.User> CreateUser(
                string auth0UserId,
                string userEmail,
                string userName,
                CancellationToken cancellationToken = default)
            {
                Shared.Models.User user;

                // Create a new user if the user doesn't exist in the server database
                CreateUserResponse createUserResponse = await _client.CreateUser(
                    new CreateUserCommand
                    {
                        Auth0Id = auth0UserId,
                        Email = userEmail,
                        UserName = userName
                    },
                    cancellationToken);

                user = await GatherUserData(createUserResponse, auth0UserId, userEmail, userName, cancellationToken);

                bool userExistsLocally = await _localContext.Users
                    .AnyAsync(u => u.Auth0Id == auth0UserId, cancellationToken);

                // Add user to the local database if the user doesn't exist locally
                if (!userExistsLocally)
                {
                    await _localContext.Users.AddAsync(user, cancellationToken);

                    await _localContext.SaveChangesAsync();

                    return user;
                }

                return user;
            }

            private async Task<Shared.Models.User> GatherUserData(
                CreateUserResponse createUserResponse,
                string auth0UserId,
                string userEmail,
                string userName,
                CancellationToken cancellationToken = default)
            {
                List<Island> islands = await _localContext.Islands
                    .Where(island => createUserResponse.User.UserIslandIds.Contains(island.Id))
                    .ToListAsync(cancellationToken);

                List<Pet> pets = await _localContext.Pets
                    .Where(pet => createUserResponse.User.UserPetIds.Contains(pet.Id))
                    .ToListAsync(cancellationToken);

                Shared.Models.User user = new()
                {
                    Id = createUserResponse.User.Id,
                    Auth0Id = auth0UserId,
                    Email = userEmail,
                    UserName = userName,
                    Balance = createUserResponse.User.Balance
                };

                // Replicate island and pet ownership from server
                foreach (Island? island in islands)
                {
                    user.Islands?.Add(new UserIsland()
                    {
                        Island = island
                    });
                }

                foreach (Pet? pet in pets)
                {
                    user.Pets?.Add(new UserPet()
                    {
                        Pet = pet
                    });
                }

                user.SelectedIsland = await GetInitialIslandQuery()
                    .FirstOrDefaultAsync(cancellationToken);

                user.SelectedPet = await GetInitialPetQuery()
                    .FirstOrDefaultAsync(cancellationToken);

                return user;
            }

            public IQueryable<Island> GetInitialIslandQuery()
            {
                return _localContext.Islands
                    .Where(island => island.Name == FocusCore.Consts.NameOfInitialIsland);
            }

            public IQueryable<Pet> GetInitialPetQuery()
            {
                return _localContext.Pets
                    .Where(pet => pet.Name == FocusCore.Consts.NameOfInitialPet);
            }
        }
    }
}
