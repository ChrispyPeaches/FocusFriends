using System.IO.Compression;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace FocusApp.Client.Methods.Badges
{
    internal class CheckSessionBadgeEligbility
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
            }

            public async Task<BadgeEligibilityResult> Handle(Query query, CancellationToken cancellationToken)
            {
                BadgeEligibilityResult result = new()
                {
                    EarnedBadge = null,
                    IsEligible = false
                };

                int sessionCount = await _localContext.UserSessionHistory
                    .CountAsync(u => u.UserId == _authenticationService.CurrentUser.Id, cancellationToken);

                var b = await _localContext.UserSessionHistory.Where(s => s.UserId == _authenticationService.CurrentUser.Id).ToListAsync(cancellationToken);

                string? badgeName = sessionCount switch
                {
                    1 => "Zen Novice",
                    10 => "Zen Apprentice",
                    100 => "Zen Master",
                    _ => null,
                };

                if (!string.IsNullOrEmpty(badgeName))
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

                    result.IsEligible = true;
                }

                var a = await _localContext.UserBadges.ToListAsync(cancellationToken);
                return result;
            }
        }
    }
}
