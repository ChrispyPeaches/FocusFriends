using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    internal class GetAndSyncUser
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

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var user = ProjectionHelper.ProjectFromBaseUser(response.Content.User);
                        result = await SyncAndGetUserData(request.Auth0Id, user, cancellationToken);
                        break;
                    case HttpStatusCode.NotFound:
                        _logger.LogDebug("User not found on server.");
                        result = await SyncAndGetUserData(request.Auth0Id, cancellationToken: cancellationToken);
                        break;
                    case HttpStatusCode.InternalServerError:
                        _logger.LogDebug("Error fetching user from server.");
                        result = await SyncAndGetUserData(request.Auth0Id, cancellationToken: cancellationToken);
                        break;
                }

                return result;
            }


            /// <summary>
            /// Gather the existing user's data from either the mobile database
            /// or the server if it isn't found in the local database.
            /// </summary>
            private async Task<Shared.Models.User?> SyncAndGetUserData(
                string auth0Id,
                Shared.Models.User? responseUser = null,
                CancellationToken cancellationToken = default)
            {
                Shared.Models.User? user;

                Shared.Models.User? localUser = await _context.Users
                    .Include(u => u.SelectedIsland)
                    .Include(u => u.SelectedPet)
                    .Include(u => u.SelectedFurniture)
                    .Include(u => u.SelectedBadge)
                    .Where(u => u.Auth0Id == auth0Id)
                    .FirstOrDefaultAsync(cancellationToken);

                // Sync user data
                if (responseUser is not null && 
                    localUser is not null)
                {
                    localUser.UserName = !string.IsNullOrEmpty(responseUser.UserName) ? responseUser.UserName : localUser.UserName;
                    localUser.FirstName = !string.IsNullOrEmpty(responseUser.FirstName) ? responseUser.FirstName : localUser.FirstName;
                    localUser.LastName = !string.IsNullOrEmpty(responseUser.LastName) ? responseUser.LastName : localUser.LastName;
                    localUser.Pronouns = !string.IsNullOrEmpty(responseUser.Pronouns) ? responseUser.Pronouns : localUser.Pronouns;

                    // Gather the user's selected island and pet or get the defaults if one isn't selected
                    localUser.SelectedIslandId ??= await _syncService
                        .GetInitialIslandQuery()
                        .Select(island => island.Id)
                        .FirstOrDefaultAsync(cancellationToken);

                    localUser.SelectedPetId ??= await _syncService
                        .GetInitialPetQuery()
                        .Select(pet => pet.Id)
                        .FirstOrDefaultAsync(cancellationToken);

                    localUser.SelectedPetId = responseUser.SelectedPetId;
                    localUser.SelectedBadgeId = responseUser.SelectedBadgeId;
                    localUser.SelectedFurnitureId = responseUser.SelectedFurnitureId;
                    localUser.SelectedIslandId = responseUser.SelectedIslandId;

                    await _context.SaveChangesAsync(cancellationToken);

                    localUser.SelectedPet ??= await _context.Pets
                        .Where(pet => pet.Id == localUser.SelectedPetId)
                        .FirstOrDefaultAsync(cancellationToken);
                    localUser.SelectedIsland ??= await _context.Islands
                        .Where(island => island.Id == localUser.SelectedIslandId)
                        .FirstOrDefaultAsync(cancellationToken);
                    localUser.SelectedBadge ??= await _context.Badges
                        .Where(badge => badge.Id == localUser.SelectedBadgeId)
                        .FirstOrDefaultAsync(cancellationToken);
                    localUser.SelectedFurniture ??= await _context.Furniture
                        .Where(decor => decor.Id == localUser.SelectedFurnitureId)
                        .FirstOrDefaultAsync(cancellationToken);
                }

                return localUser;
            }
        }

    }
}
