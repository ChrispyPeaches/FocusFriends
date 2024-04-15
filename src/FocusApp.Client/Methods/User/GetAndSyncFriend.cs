using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusCore.Queries.User;
using FocusCore.Responses.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Refit;

namespace FocusApp.Client.Methods.User
{
    internal class GetAndSyncFriend
    {
        public class Query : IRequest<Shared.Models.User?>
        {
            public string Auth0Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Shared.Models.User?>
        {
            private readonly IAPIClient _client;
            private readonly FocusAppContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly ISyncService _syncService;

            public Handler(
                IAPIClient client,
                FocusAppContext context,
                ILogger<Handler> logger,
                ISyncService syncService)
            {
                _client = client;
                _context = context;
                _logger = logger;
                _syncService = syncService;
            }

            public async Task<Shared.Models.User?> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                Shared.Models.User? result = null;
                // Fetch user data from the server
                ApiResponse<GetUserResponse>? response = null;
                try
                {
                    response = await _client.GetUserByAuth0Id(
                        new GetUserQuery
                        {
                            Auth0Id = request.Auth0Id
                        },
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching user from server", ex);
                }

                try
                {
                    Shared.Models.User? user = null;
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            user = ProjectionHelper.ProjectFromBaseUser(response.Content.User);
                            break;
                        case HttpStatusCode.NotFound:
                            _logger.LogDebug(response.Error, "User not found on server.");
                            break;
                        case HttpStatusCode.InternalServerError:
                            _logger.LogDebug(response.Error, "Error fetching user from server.");
                            break;
                    }

                    result = await SyncAndGetUserData(request.Auth0Id, user, cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching user from server", ex);
                }

                return result;
            }


            /// <summary>
            /// Gather the existing user's data from either the mobile database
            /// or the server if it isn't found in the local database.
            /// </summary>
            private async Task<Shared.Models.User?> SyncAndGetUserData(
                string auth0Id,
                Shared.Models.User? responseFriend = null,
                CancellationToken cancellationToken = default)
            {
                Shared.Models.User? localFriend = await _context.Users
                    .Include(u => u.SelectedIsland)
                    .Include(u => u.SelectedPet)
                    .Include(u => u.SelectedFurniture)
                    .Include(u => u.SelectedBadge)
                    .Where(u => u.Auth0Id == auth0Id)
                    .FirstOrDefaultAsync(cancellationToken);

                
                // Sync user data
                if (responseFriend is null && localFriend is null)
                {
                    throw new InvalidOperationException("User data not found.");
                }
                else if (responseFriend is null || localFriend is null)
                {
                    localFriend ??= responseFriend;
                }
                else
                {
                    await UpdateLocalFriendWithServerInfo(localFriend, responseFriend, cancellationToken);
                }

                // If the selected island or pet are null, set to default
                localFriend.SelectedIsland ??= await _syncService
                    .GetInitialIslandQuery()
                    .FirstOrDefaultAsync(cancellationToken);
                localFriend.SelectedIslandId ??= localFriend.SelectedIsland?.Id;

                localFriend.SelectedPet ??= await _syncService
                    .GetInitialPetQuery()
                    .FirstOrDefaultAsync(cancellationToken);
                localFriend.SelectedPetId ??= localFriend.SelectedPet?.Id;

                if (_context.ChangeTracker.HasChanges())
                    await _context.SaveChangesAsync(cancellationToken);

                return localFriend;
            }

            private async Task UpdateLocalFriendWithServerInfo(
                Shared.Models.User? localFriend,
                Shared.Models.User? responseFriend,
                CancellationToken cancellationToken)
            {
                // Update normal fields
                localFriend.UserName = CheckNullAndUpdateString(localFriend.UserName, responseFriend.UserName);
                localFriend.FirstName = CheckNullAndUpdateString(localFriend.FirstName, responseFriend.FirstName);
                localFriend.LastName = CheckNullAndUpdateString(localFriend.LastName, responseFriend.LastName);
                localFriend.Pronouns = CheckNullAndUpdateString(localFriend.Pronouns, responseFriend.Pronouns);

                localFriend.ProfilePicture = responseFriend.ProfilePicture ?? localFriend.ProfilePicture;

                // Update selected items
                if (responseFriend.SelectedPetId != null &&
                    localFriend.SelectedPetId != responseFriend.SelectedPetId)
                {
                    localFriend.SelectedPetId = responseFriend.SelectedPetId;
                    localFriend.SelectedPet = await _context.Pets
                        .Where(pet => pet.Id == localFriend.SelectedPetId)
                        .FirstOrDefaultAsync(cancellationToken);
                }

                if (responseFriend.SelectedIslandId != null &&
                    localFriend.SelectedIslandId != responseFriend.SelectedIslandId)
                {
                    localFriend.SelectedIslandId = responseFriend.SelectedIslandId;
                    localFriend.SelectedIsland = await _context.Islands
                        .Where(island => island.Id == localFriend.SelectedIslandId)
                        .FirstOrDefaultAsync(cancellationToken);
                }

                if (responseFriend.SelectedBadgeId != null &&
                    localFriend.SelectedBadgeId != responseFriend.SelectedBadgeId)
                {
                    localFriend.SelectedBadgeId = responseFriend.SelectedBadgeId;
                    localFriend.SelectedBadge = await _context.Badges
                        .Where(badge => badge.Id == localFriend.SelectedBadgeId)
                        .FirstOrDefaultAsync(cancellationToken);
                }

                if (responseFriend.SelectedFurnitureId != null &&
                    localFriend.SelectedFurnitureId != responseFriend.SelectedFurnitureId)
                {
                    localFriend.SelectedFurnitureId = responseFriend.SelectedFurnitureId;
                    localFriend.SelectedFurniture = await _context.Furniture
                        .Where(furniture => furniture.Id == localFriend.SelectedFurnitureId)
                        .FirstOrDefaultAsync(cancellationToken);
                }
            }

            private static string CheckNullAndUpdateString(string existingValue, string newValue) => 
                !string.IsNullOrEmpty(newValue) ? newValue : existingValue;
        }

    }
}
