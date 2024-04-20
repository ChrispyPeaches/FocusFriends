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

namespace FocusApp.Client.Helpers
{
    internal class BadgeService
    {
        IMediator _mediator;
        public BadgeService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<BadgeEligibilityResult> CheckPurchaseBadgeEligibility(ShopItem item, CancellationToken cancellationToken)
        {
            BadgeEligibilityResult result = new();

            switch (item.Type)
            {
                case ShopItemType.Pets:
                    result = await _mediator.Send(new CheckPetPurchaseBadgeEligbility.Query(), cancellationToken);
                    break;
                case ShopItemType.Islands:
                    result = await _mediator.Send(new CheckIslandPurchaseBadgeEligbility.Query(), cancellationToken);
                    break;
                case ShopItemType.Decor:
                    result = await _mediator.Send(new CheckDecorPurchaseBadgeEligbility.Query(), cancellationToken);
                    break;
                default:
                    break;
            }

            return result;
        }   
    }
}
