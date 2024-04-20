using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocusApp.Shared.Data;
using FocusCore.Models;
using FocusApp.Shared.Models;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Client.Helpers
{
    internal class BadgeService
    {
        FocusAppContext _localContext;
        IAuthenticationService _authenticationService;
        public BadgeService(FocusAppContext localContext, IAuthenticationService authenticationService)
        {
            _localContext = localContext;
            _authenticationService = authenticationService;
        }

        public async Task<BadgeEligibilityResult> CheckPurchaseBadgeEligibility(ShopItem item, CancellationToken cancellationToken)
        {
            BadgeEligibilityResult result = new();
            switch (item.Type)
            {
                case ShopItemType.Pets:
                    result = await CheckPetPurchaseBadgeEligibility(item, cancellationToken);
                    break;
                case ShopItemType.Islands:
                    break;
                case ShopItemType.Decor:
                    break;
                default:
                    result = new BadgeEligibilityResult();
                    break;
            }

            return result;
        }

        private async Task<BadgeEligibilityResult> CheckPetPurchaseBadgeEligibility(ShopItem item, CancellationToken cancellationToken)
        {
            User? user = await _localContext.Users
                .Include(u => u.Pets)
                .SingleOrDefaultAsync(u => u.Id == _authenticationService.CurrentUser.Id, cancellationToken);

            if (user == null)
                throw new InvalidOperationException("User not found in local database.");

            BadgeEligibilityResult result = new();

            if (user.Pets?.Count == 1)
            {
                Badge companionCollectorBadge = await _localContext.Badges.SingleAsync(u => u.Name == "CompanionCollector", cancellationToken);
                user.Badges?.Add(new UserBadge { Badge = companionCollectorBadge });

                result.IsEligible = true;
                result.EarnedBadge = companionCollectorBadge;
            }
            else if (user.Pets.Count == await _localContext.Pets.CountAsync(cancellationToken))
            { 
                Badge petParadiseBadge = await _localContext.Badges.SingleAsync(u => u.Name == "PetParadise", cancellationToken);
                user.Badges?.Add(new UserBadge { Badge = petParadiseBadge });

                result.IsEligible = true;
                result.EarnedBadge = petParadiseBadge;
            }

            return result;
        }
    }
}
