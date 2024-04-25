using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using MediatR;
using FocusCore.Commands.User;
using FocusApp.Client.Clients;
using Microsoft.Extensions.Logging;

namespace FocusApp.Client.Methods.Badges
{
    internal class CheckIslandPurchaseBadgeEligbility
    {
        public class Query : IRequest<BadgeEligibilityResult> { }
        public class Handler : IRequestHandler<Query, BadgeEligibilityResult>
        {
            FocusAppContext _localContext;
            IAuthenticationService _authenticationService;
            IAPIClient _client;
            ILogger<CheckIslandPurchaseBadgeEligbility> _logger;
            public Handler(FocusAppContext localContext, IAuthenticationService authenticationService, IAPIClient client, ILogger<CheckIslandPurchaseBadgeEligbility> logger)
            {
                _localContext = localContext;
                _authenticationService = authenticationService;
                _client = client;
                _logger = logger;
            }

            public async Task<BadgeEligibilityResult> Handle(Query query, CancellationToken cancellationToken)
            {
                Shared.Models.User? user = await _localContext.Users
                    .Include(u => u.Islands)
                    .SingleOrDefaultAsync(u => u.Id == _authenticationService.CurrentUser.Id, cancellationToken);

                if (user == null)
                    throw new InvalidOperationException("User not found in local database.");

                BadgeEligibilityResult result = new();

                // Count = 2 here because user starts owning one island (Tropical)
                if (user.Islands?.Count == 2)
                {
                    result.IsEligible = true;

                    Badge islandVoyagerBadge = await _localContext.Badges.SingleAsync(u => u.Name == "Island Voyager", cancellationToken);
                    user.Badges?.Add(new UserBadge { Badge = islandVoyagerBadge, DateAcquired = DateTime.UtcNow });
                    result.EarnedBadge = islandVoyagerBadge;
                }
                else if (user.Islands?.Count == await _localContext.Islands.CountAsync(cancellationToken))
                {
                    result.IsEligible = true;

                    Badge globalIconBadge = await _localContext.Badges.SingleAsync(u => u.Name == "Global Icon", cancellationToken);
                    user.Badges?.Add(new UserBadge { Badge = globalIconBadge, DateAcquired = DateTime.UtcNow });
                    result.EarnedBadge = globalIconBadge;
                }

                if (result.IsEligible)
                {
                    // Save new user badge to local database
                    await _localContext.SaveChangesAsync(cancellationToken);

                    // Save new user badge to server database
                    try
                    {
                        await _client.AddUserBadge(new AddUserBadgeCommand { UserId = _authenticationService.CurrentUser.Id, BadgeId = result.EarnedBadge.Id });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while adding user badge on server.");
                    }
                }

                return result;
            }
        }
    }
}
