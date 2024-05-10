using System.Net;
using System.Security.Claims;
using Auth0.OidcClient;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Methods.Sync;
using FocusApp.Client.Views;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using FocusCore.Queries.User;
using FocusCore.Responses.User;
using IdentityModel.OidcClient;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Refit;

namespace FocusApp.Client.Methods.User
{
    internal class GetUserLogin
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
            IMediator _mediator;
            private readonly IAuthenticationService _authService;
            private readonly PopupService _popupService;

            public Handler(
                Auth0Client auth0Client,
                IAPIClient client,
                FocusAppContext localContext,
                ILogger<Handler> logger,
                IMediator mediator,
                IAuthenticationService authService,
                PopupService popupService)
            {
                _auth0Client = auth0Client;
                _client = client;
                _localContext = localContext;
                _logger = logger;
                _mediator = mediator;
                _authService = authService;
                _popupService = popupService;
            }

            public async Task<Result> Handle(
                Query query,
                CancellationToken cancellationToken = default)
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
                                case HttpStatusCode.OK:
                                    user = await GatherExistingUserData(response.Content, auth0UserId, cancellationToken);
                                    break;
                                case HttpStatusCode.NotFound:
                                    user = await CreateUser(auth0UserId, userEmail, userName, cancellationToken);
                                    break;
                                case HttpStatusCode.InternalServerError:
                                default:
                                    throw new Exception("Error fetching user from server.", response.Error);
                            }

                            await SecureStorage.Default.SetAsync("access_token", loginResult.AccessToken);
                            await SecureStorage.Default.SetAsync("id_token", loginResult.IdentityToken);

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

                user = await GatherUserDataForCreatedUser(createUserResponse, auth0UserId, userEmail, userName, cancellationToken);

                bool userExistsLocally = await _localContext.Users
                    .AnyAsync(u => u.Auth0Id == auth0UserId || u.Id == user.Id, cancellationToken);

                // Add user to the local database if the user doesn't exist locally
                if (!userExistsLocally)
                {
                    await _localContext.Users.AddAsync(user, cancellationToken);

                    await _localContext.SaveChangesAsync();

                    return user;
                }

                return user;
            }

            private async Task<Shared.Models.User> GatherUserDataForCreatedUser(
                CreateUserResponse createUserResponse,
                string auth0UserId,
                string userEmail,
                string userName,
                CancellationToken cancellationToken = default)
            {
                if (createUserResponse.User == null)
                {
                    return null;
                }

                Shared.Models.User user = new()
                {
                    Id = createUserResponse.User.Id,
                    Auth0Id = auth0UserId,
                    Email = userEmail,
                    UserName = userName,
                    Balance = createUserResponse.User.Balance,
                    // Note: DateCreated is the UTC DateTime the user was created (synced with server record)
                    DateCreated = createUserResponse.User.DateCreated.UtcDateTime
                };

                user.SelectedIsland = await GetInitialIslandQuery()
                    .FirstOrDefaultAsync(cancellationToken);
                user.Islands?.Add(new UserIsland()
                {
                    DateAcquired = DateTime.UtcNow,
                    Island = user.SelectedIsland
                });

                user.SelectedPet = await GetInitialPetQuery()
                    .FirstOrDefaultAsync(cancellationToken);
                user.Pets?.Add(new UserPet()
                {
                    DateAcquired = DateTime.UtcNow,
                    Pet = user.SelectedPet
                });

                return user;
            }

            /// <summary>
            /// Gather the existing user's data from either the mobile database
            /// or the server if it isn't found in the local database.
            /// </summary>
            private async Task<Shared.Models.User?> GatherExistingUserData(
                GetUserResponse? getUserResponse,
                string auth0Id,
                CancellationToken cancellationToken = default)
            {
                Shared.Models.User? user;

                if (getUserResponse?.User is null)
                {
                    Shared.Models.User? localUser = await _localContext.Users
                        .Include(u => u.SelectedIsland)
                        .Include(u => u.SelectedPet)
                        .Where(u => u.Auth0Id == auth0Id)
                        .FirstOrDefaultAsync(cancellationToken);

                    user = localUser;
                }
                else
                {
                    user = ProjectionHelper.ProjectFromBaseUser(getUserResponse.User);

                    bool userExistsLocally = await _localContext.Users
                        .AnyAsync(u => u.Auth0Id == getUserResponse.User.Auth0Id || getUserResponse.User.Id == u.Id, cancellationToken);
                    
                    // If the user doesn't exist locally and they have content that isnt't in the DB,
                    // wait for all content to be downloaded to ensure the app has all of their items downloaded
                    if (!userExistsLocally &&
                        _authService.StartupSyncTask != null &&
                        !_authService.StartupSyncTask.IsCompleted &&
                        await DoesUserHaveUnsyncedData(getUserResponse))
                    {
                        await _popupService.ShowPopupAsync<SyncDataLoadingPopupInterface>();
                        await _authService.StartupSyncTask.WaitAsync(cancellationToken);
                        await _popupService.HidePopupAsync<SyncDataLoadingPopupInterface>();
                    }

                    // Gather the user's selected island and pet or get the defaults if one isn't selected
                    user.SelectedIsland = user.SelectedIslandId == null ?
                        // If the user does not have a selected island id, default to tropical
                        await GetInitialIslandQuery().FirstOrDefaultAsync(cancellationToken) 
                        :
                        await GetSelectedIslandQuery(user.SelectedIslandId.Value)
                            .FirstOrDefaultAsync(cancellationToken);

                    user.SelectedPet = user.SelectedPetId == null ?
                        // If the user does not have a selected pet id, default to cool cat
                        await GetInitialPetQuery().FirstOrDefaultAsync(cancellationToken)
                        :
                        await GetSelectedPetQuery(user.SelectedPetId.Value)
                            .FirstOrDefaultAsync(cancellationToken);

                    user.SelectedBadge = user.SelectedBadgeId == null ?
                        null
                        :
                        await GetSelectedBadgeQuery(user.SelectedBadgeId.Value)
                            .FirstOrDefaultAsync(cancellationToken);

                    user.SelectedDecor = user.SelectedDecorId == null ?
                        null
                        :
                        await GetSelectedDecorQuery(user.SelectedDecorId.Value)
                            .FirstOrDefaultAsync(cancellationToken);

                    
                    // Add user to the local database if the user doesn't exist locally
                    if (!userExistsLocally)
                    {
                        await _localContext.Users.AddAsync(user, cancellationToken);

                        await _localContext.SaveChangesAsync();
                    }

                    try
                    {
                        // Sync local user data with server user data
                        await _mediator.Send(new SyncUserData.Query
                        {
                            ServerUser = user,
                            UserIslandIds = getUserResponse.UserIslandIds,
                            UserPetIds = getUserResponse.UserPetIds,
                            UserDecorIds = getUserResponse.UserDecorIds,
                            UserBadgeIds = getUserResponse.UserBadgeIds
                        }, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error syncing local user data with server's user data.");
                    }
                }

                return user;
            }

            /// <summary>
            /// Check if the user owns any items that aren't in the mobile DB
            /// </summary>
            private async Task<bool> DoesUserHaveUnsyncedData(GetUserResponse response)
            {
                var mobileDbHasAllOwnedIslands = (await _localContext.Islands.Where(i => response.UserIslandIds.Contains(i.Id)).CountAsync()) ==
                                                response.UserIslandIds.Count;
                if (!mobileDbHasAllOwnedIslands) return true;

                var mobileDbHasAllOwnedPets = (await _localContext.Pets.Where(p => response.UserPetIds.Contains(p.Id)).CountAsync()) ==
                                                response.UserPetIds.Count;
                if (!mobileDbHasAllOwnedPets) return true;

                var mobileDbHasAllOwnedDecor = (await _localContext.Decor.Where(d => response.UserDecorIds.Contains(d.Id)).CountAsync()) ==
                                                response.UserDecorIds.Count;
                if (!mobileDbHasAllOwnedDecor) return true;


                var mobileDbHasAllOwnedBadges = (await _localContext.Badges.Where(b => response.UserBadgeIds.Contains(b.Id)).CountAsync()) ==
                                                response.UserBadgeIds.Count;
                if (!mobileDbHasAllOwnedBadges) return true;

                return false;
            }

            private IQueryable<Island> GetInitialIslandQuery()
            {
                return _localContext.Islands
                    .Where(island => island.Name == FocusCore.Consts.NameOfInitialIsland);
            }

            private IQueryable<Pet> GetInitialPetQuery()
            {
                return _localContext.Pets
                    .Where(pet => pet.Name == FocusCore.Consts.NameOfInitialPet);
            }

            private IQueryable<Island> GetSelectedIslandQuery(Guid selectedIslandId)
            {
                return _localContext.Islands
                    .Where(island => island.Id == selectedIslandId);
            }

            private IQueryable<Pet> GetSelectedPetQuery(Guid selectedPetId)
            {
                return _localContext.Pets
                    .Where(pet => pet.Id == selectedPetId);
            }

            private IQueryable<Badge> GetSelectedBadgeQuery(Guid selectedBadgeId)
            {
                return _localContext.Badges
                    .Where(badge => badge.Id == selectedBadgeId);
            }

            private IQueryable<Decor> GetSelectedDecorQuery(Guid selectedDecorId)
            {
                return _localContext.Decor
                    .Where(decor => decor.Id == selectedDecorId);
            }
        }
    }
}
