using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace FocusApp.Client.Methods.Badges
{
    internal class CheckPetPurchaseBadgeEligbility
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

                return result;
            }
        }
    }
}
