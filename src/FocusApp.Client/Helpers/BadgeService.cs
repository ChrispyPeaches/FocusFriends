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
using MediatR;
using FocusApp.Client.Methods.Badges;
using Microsoft.Extensions.Logging;

namespace FocusApp.Client.Helpers
{
    internal interface IBadgeService
    {
        Task<BadgeEligibilityResult> CheckPurchaseBadgeEligibility(ShopItem item, CancellationToken cancellationToken);
        Task<BadgeEligibilityResult> CheckSessionBadgeEligability(CancellationToken cancellationToken);
        Task<BadgeEligibilityResult> CheckSocialBadgeEligability(CancellationToken cancellationToken);
    }

    internal class BadgeService : IBadgeService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BadgeService> _logger;
        public BadgeService(
            IMediator mediator, 
            ILogger<BadgeService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<BadgeEligibilityResult> CheckPurchaseBadgeEligibility(ShopItem item, CancellationToken cancellationToken)
        {
            BadgeEligibilityResult result = new();

            switch (item.Type)
            {
                case ShopItemType.Pets:
                    try
                    {
                        result = await _mediator.Send(new CheckPetPurchaseBadgeEligbility.Query(), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while checking for pet purchase badge eligibility.");
                    }
                    break;
                case ShopItemType.Islands:
                    try
                    {
                        result = await _mediator.Send(new CheckIslandPurchaseBadgeEligbility.Query(), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while checking for island purchase badge eligibility.");
                    }
                    break;
                case ShopItemType.Decor:
                    try
                    {
                        result = await _mediator.Send(new CheckDecorPurchaseBadgeEligbility.Query(), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while checking for decor purchase badge eligibility.");
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        public async Task<BadgeEligibilityResult> CheckSessionBadgeEligability(CancellationToken cancellationToken)
        {
            BadgeEligibilityResult result = new();

            try
            {
                result = await _mediator.Send(new CheckSessionBadgeEligbility.Query(), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when checking session badge eligibility.");
            }

            return result;
        }

        public async Task<BadgeEligibilityResult> CheckSocialBadgeEligability(CancellationToken cancellationToken)
        {
            return new BadgeEligibilityResult();
        }
    }
}
