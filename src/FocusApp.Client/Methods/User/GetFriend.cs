using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using FluentValidation;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusCore.Commands.User;
using FocusCore.Queries.User;
using FocusCore.Responses.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Refit;

namespace FocusApp.Client.Methods.User
{
    internal class GetFriend
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(user => user.Auth0Id)
                    .NotNull()
                    .NotEmpty();
            }
        }

        public class Query : IRequest<Shared.Models.User?>
        {
            public string? Auth0Id { get; set; }
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
                            if (response.Content?.User is not null)
                                user = ProjectionHelper.ProjectFromBaseUser(response.Content.User);
                            break;
                        case HttpStatusCode.NotFound:
                            _logger.LogDebug(response.Error, "User not found on server.");
                            break;
                        case HttpStatusCode.InternalServerError:
                            _logger.LogDebug(response.Error, "Error fetching user from server.");
                            break;
                    }

                    if (user is not null)
                    {
                        return await GetFriendData(user, cancellationToken);
                    }
                    else
                    {
                        throw new NullReferenceException("User in response was null.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching user from server", ex);
                }
            }


            /// <summary>
            /// Gather the friend's selected items.
            /// </summary>
            private async Task<Shared.Models.User?> GetFriendData(
                Shared.Models.User responseFriend,
                CancellationToken cancellationToken = default)
            {
                // If the selected island or pet are null, set to default
                responseFriend.SelectedIsland = responseFriend.SelectedIslandId is null ?
                    await _syncService
                        .GetInitialIslandQuery()
                        .FirstOrDefaultAsync(cancellationToken)
                    : await _context.Islands
                        .Where(island => island.Id == responseFriend.SelectedIslandId)
                        .FirstOrDefaultAsync(cancellationToken);

                responseFriend.SelectedPet = responseFriend.SelectedPetId is null ?
                    await _syncService
                        .GetInitialPetQuery()
                        .FirstOrDefaultAsync(cancellationToken)
                    : await _context.Pets
                        .Where(pet => pet.Id == responseFriend.SelectedPetId)
                        .FirstOrDefaultAsync(cancellationToken);

                // Gather badge and decor if the friend has selected them
                if (responseFriend.SelectedDecorId is not null)
                {
                    responseFriend.SelectedDecor = await _context.Decor
                        .Where(decor => decor.Id == responseFriend.SelectedDecorId)
                        .FirstOrDefaultAsync(cancellationToken);
                }

                if (responseFriend.SelectedBadgeId is not null)
                {
                    responseFriend.SelectedBadge = await _context.Badges
                        .Where(badge => badge.Id == responseFriend.SelectedBadgeId)
                        .FirstOrDefaultAsync(cancellationToken);
                }

                return responseFriend;
            }
        }
    }
}
