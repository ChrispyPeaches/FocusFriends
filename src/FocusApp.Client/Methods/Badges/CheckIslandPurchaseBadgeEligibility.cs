using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace FocusApp.Client.Methods.Badges
{
    internal class CheckIslandPurchaseBadgeEligbility
    {
        public class Query : IRequest<BadgeEligibilityResult> { }
        public class Handler : IRequestHandler<Query, BadgeEligibilityResult>
        {
            FocusAppContext _localContext;
            IAuthenticationService _authenticationService;
            public Handler(FocusAppContext localContext, IAuthenticationService authenticationService)
            {
                _localContext = localContext;
                _authenticationService = authenticationService;
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
                    user.Badges?.Add(new UserBadge { Badge = islandVoyagerBadge });
                    result.EarnedBadge = islandVoyagerBadge;
                }
                else if (user.Islands?.Count == await _localContext.Islands.CountAsync(cancellationToken))
                {
                    result.IsEligible = true;

                    Badge globalIconBadge = await _localContext.Badges.SingleAsync(u => u.Name == "Global Icon", cancellationToken);
                    user.Badges?.Add(new UserBadge { Badge = globalIconBadge });
                    result.EarnedBadge = globalIconBadge;
                }

                return result;
            }
        }
    }
}
