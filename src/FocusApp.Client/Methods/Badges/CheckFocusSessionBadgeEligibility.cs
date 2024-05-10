using System.IO.Compression;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace FocusApp.Client.Methods.Badges
{
    internal class CheckFocusSessionBadgeEligibility
    {
        public class Query : IRequest<BadgeEligibilityResult> { }

        public class Handler : IRequestHandler<Query, BadgeEligibilityResult>
        {
            private readonly FocusAppContext _localContext;
            private readonly IAuthenticationService _authenticationService;
            private readonly IAPIClient _client;

            public Handler(FocusAppContext localContext, IAuthenticationService authenticationService, IAPIClient client)
            {
                _localContext = localContext;
                _authenticationService = authenticationService;
                _client = client;
            }

            public async Task<BadgeEligibilityResult> Handle(Query query, CancellationToken cancellationToken)
            {
                BadgeEligibilityResult result = new()
                {
                    EarnedBadge = null,
                    IsEligible = false
                };

                if (!_authenticationService.IsLoggedIn)
                {
                    throw new InvalidOperationException("User is not logged in.");
                }

                int sessionCount = await _localContext.UserSessionHistory
                    .CountAsync(u => u.UserId == _authenticationService.Id.Value, cancellationToken);

                string? badgeName = sessionCount switch
                {
                    1 => "Zen Novice",
                    10 => "Zen Apprentice",
                    100 => "Zen Master",
                    _ => null,
                };

                await AddBadgeToUser(badgeName, result, cancellationToken);

                return result;
            }

            private async Task AddBadgeToUser(string? badgeName, BadgeEligibilityResult result, CancellationToken cancellationToken)
            {
                if (!string.IsNullOrEmpty(badgeName))
                {
                    bool hasBadge = await _localContext.UserBadges
                        .Where(userBadge =>
                            userBadge.UserId == _authenticationService.Id.Value &&
                            userBadge.Badge.Name == badgeName)
                        .AnyAsync(cancellationToken);

                    if (!hasBadge)
                    {
                        result.EarnedBadge = await _localContext.Badges
                            .SingleAsync(u => u.Name == badgeName, cancellationToken);

                        _localContext.UserBadges.Add(new UserBadge()
                        {
                            BadgeId = result.EarnedBadge.Id,
                            UserId = _authenticationService.Id.Value,
                            DateAcquired = DateTime.UtcNow
                        });

                        if (_localContext.ChangeTracker.HasChanges())
                            await _localContext.SaveChangesAsync(cancellationToken);

                        await _client.AddUserBadge(new AddUserBadgeCommand()
                            {
                                BadgeId = result.EarnedBadge.Id,
                                UserId = _authenticationService.Id.Value
                            },
                            cancellationToken);

                        result.IsEligible = true;
                    }
                }
            }
        }
    }
}
