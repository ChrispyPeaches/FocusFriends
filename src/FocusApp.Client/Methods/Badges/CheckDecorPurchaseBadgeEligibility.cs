using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace FocusApp.Client.Methods.Badges
{
    internal class CheckDecorPurchaseBadgeEligbility
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

                if (user.Decor?.Count == 1)
                {
                    result.IsEligible = true;

                    Badge interiorDesignerBadge = await _localContext.Badges.SingleAsync(u => u.Name == "Interior Designer", cancellationToken);
                    user.Badges?.Add(new UserBadge { Badge = interiorDesignerBadge });
                    result.EarnedBadge = interiorDesignerBadge;
                }
                else if (user.Decor?.Count == await _localContext.Decor.CountAsync(cancellationToken))
                {
                    result.IsEligible = true;

                    Badge decorDynastyBadge = await _localContext.Badges.SingleAsync(u => u.Name == "Decor Dynasty", cancellationToken);
                    user.Badges?.Add(new UserBadge { Badge = decorDynastyBadge });
                    result.EarnedBadge = decorDynastyBadge;
                }

                return result;
            }
        }
    }
}
