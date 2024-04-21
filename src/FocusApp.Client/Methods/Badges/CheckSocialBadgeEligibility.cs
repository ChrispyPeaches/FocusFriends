using FocusApp.Client.Clients;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using FocusCore.Queries.Social;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace FocusApp.Client.Methods.Badges
{
    internal class CheckSocialBadgeEligibility
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
                BadgeEligibilityResult result = new();

                var friendResult = await _client
                        .GetAllFriends(new GetAllFriendsQuery()
                        {
                            UserId = _authenticationService.CurrentUser.Id
                        },
                        cancellationToken);

                if (friendResult == null)
                {
                    throw new NullReferenceException("Get all friends request failed.");
                }

                int friendCount = friendResult.Count;

                string? badgeName = friendCount switch
                {
                    (>= 1) => "Better Together",
                    _ => null,
                };

                if (!string.IsNullOrEmpty(badgeName))
                {
                    bool hasBadge = await _localContext.UserBadges
                        .Where(userBadge =>
                            userBadge.UserId == _authenticationService.CurrentUser.Id &&
                            userBadge.Badge.Name == badgeName)
                        .AnyAsync(cancellationToken);

                    if (!hasBadge)
                    {
                        await AddBadgeToUser(result, badgeName, cancellationToken);

                        result.IsEligible = true;
                    }
                }

                var a = await _localContext.UserBadges.ToListAsync(cancellationToken);
                return result;
            }

            public async Task AddBadgeToUser(
                BadgeEligibilityResult result,
                string badgeName,
                CancellationToken cancellationToken)
            {
                result.EarnedBadge = await _localContext.Badges
                    .SingleAsync(u => u.Name == badgeName, cancellationToken);

                _localContext.UserBadges.Add(new UserBadge()
                {
                    BadgeId = result.EarnedBadge.Id,
                    UserId = _authenticationService.CurrentUser.Id,
                    DateAcquired = DateTime.UtcNow
                });

                if (_localContext.ChangeTracker.HasChanges())
                    await _localContext.SaveChangesAsync(cancellationToken);

                await _client.AddUserBadge(new AddUserBadgeCommand()
                    {
                        BadgeId = result.EarnedBadge.Id,
                        UserId = _authenticationService.CurrentUser.Id
                    },
                    cancellationToken);
            }
        }
    }
}
