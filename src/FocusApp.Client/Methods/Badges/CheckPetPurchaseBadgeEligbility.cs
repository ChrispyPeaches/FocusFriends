using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using MediatR;
using FocusApp.Client.Clients;
using FocusCore.Commands.User;
using ThreadNetwork;
using Microsoft.Extensions.Logging;

namespace FocusApp.Client.Methods.Badges
{
    internal class CheckPetPurchaseBadgeEligbility
    {
        public class Query : IRequest<BadgeEligibilityResult> { }
        public class Handler : IRequestHandler<Query, BadgeEligibilityResult>
        {
            FocusAppContext _localContext;
            IAuthenticationService _authenticationService;
            IAPIClient _client;
            ILogger<CheckPetPurchaseBadgeEligbility> _logger;
            public Handler(FocusAppContext localContext, IAuthenticationService authenticationService, IAPIClient client, ILogger<CheckPetPurchaseBadgeEligbility> logger)
            {
                _localContext = localContext;
                _authenticationService = authenticationService;
                _client = client;
                _logger = logger;
            }

            public async Task<BadgeEligibilityResult> Handle(Query query, CancellationToken cancellationToken)
            {
                Shared.Models.User? user = await _localContext.Users
                    .Include(u => u.Pets)
                    .SingleOrDefaultAsync(u => u.Id == _authenticationService.CurrentUser.Id, cancellationToken);

                if (user == null)
                    throw new InvalidOperationException("User not found in local database.");

                BadgeEligibilityResult result = new();

                // Count = 2 here because user starts owning one island (Cool Cat)
                if (user.Pets?.Count == 2)
                {
                    Badge companionCollectorBadge = await _localContext.Badges.SingleAsync(u => u.Name == "Companion Collector", cancellationToken);
                    user.Badges?.Add(new UserBadge { Badge = companionCollectorBadge });

                    result.IsEligible = true;
                    result.EarnedBadge = companionCollectorBadge;
                }
                else if (user.Pets?.Count == await _localContext.Pets.CountAsync(cancellationToken))
                {
                    Badge petParadiseBadge = await _localContext.Badges.SingleAsync(u => u.Name == "Pet Paradise", cancellationToken);
                    user.Badges?.Add(new UserBadge { Badge = petParadiseBadge });

                    result.IsEligible = true;
                    result.EarnedBadge = petParadiseBadge;
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
