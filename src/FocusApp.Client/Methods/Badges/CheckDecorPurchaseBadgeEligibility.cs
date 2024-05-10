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
    internal class CheckDecorPurchaseBadgeEligbility
    {
        public class Query : IRequest<BadgeEligibilityResult> { }
        public class Handler : IRequestHandler<Query, BadgeEligibilityResult>
        {
            FocusAppContext _localContext;
            IAuthenticationService _authenticationService;
            IAPIClient _client;
            ILogger<CheckDecorPurchaseBadgeEligbility> _logger;
            public Handler(FocusAppContext localContext, IAuthenticationService authenticationService, IAPIClient client, ILogger<CheckDecorPurchaseBadgeEligbility> logger)
            {
                _localContext = localContext;
                _authenticationService = authenticationService;
                _client = client;
                _logger = logger;
            }

            public async Task<BadgeEligibilityResult> Handle(Query query, CancellationToken cancellationToken)
            {
                Shared.Models.User? user = await _localContext.Users
                    .Include(u => u.Decor)
                    .SingleOrDefaultAsync(u => u.Id == _authenticationService.Id.Value, cancellationToken);

                if (user == null)
                    throw new InvalidOperationException("User not found in local database.");

                BadgeEligibilityResult result = new();

                if (user.Decor?.Count == 1)
                {
                    result.IsEligible = true;

                    Badge interiorDesignerBadge = await _localContext.Badges.SingleAsync(u => u.Name == "Interior Designer", cancellationToken);
                    user.Badges?.Add(new UserBadge { Badge = interiorDesignerBadge, DateAcquired = DateTime.UtcNow });
                    result.EarnedBadge = interiorDesignerBadge;
                }
                else if (user.Decor?.Count == await _localContext.Decor.CountAsync(cancellationToken))
                {
                    result.IsEligible = true;

                    Badge decorDynastyBadge = await _localContext.Badges.SingleAsync(u => u.Name == "Decor Dynasty", cancellationToken);
                    user.Badges?.Add(new UserBadge { Badge = decorDynastyBadge, DateAcquired = DateTime.UtcNow });
                    result.EarnedBadge = decorDynastyBadge;
                }

                if (result.IsEligible)
                {
                    // Save new user badge to local database
                    await _localContext.SaveChangesAsync(cancellationToken);

                    // Save new user badge to server database
                    try
                    {
                        await _client.AddUserBadge(new AddUserBadgeCommand { UserId = _authenticationService.Id.Value, BadgeId = result.EarnedBadge.Id });
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
