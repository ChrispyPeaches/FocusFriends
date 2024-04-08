using FocusAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FocusCore.Queries.Sync;
using FocusCore.Responses.Sync;

namespace FocusAPI.Methods.Sync
{
    public class SyncBadges
    {
        public class Validator : AbstractValidator<SyncBadgesQuery>
        {
            public Validator()
            {
                RuleFor(query => query.BadgeIds)
                    .NotNull();
            }
        }

        public class Handler : IRequestHandler<SyncBadgesQuery, SyncBadgesResponse>
        {
            private readonly FocusContext _context;

            public Handler(FocusContext context)
            {
                _context = context;
            }

            public async Task<SyncBadgesResponse> Handle(
                SyncBadgesQuery query,
                CancellationToken cancellationToken)
            {
                List<Guid> serverIds = await GetServerMindfulnessTipIds(cancellationToken);
                IList<Guid> mobileIds = query.BadgeIds;

                List<FocusCore.Models.BaseBadge> missingBadges = new();
                if (serverIds.Count != 0)
                {
                    List<Guid> missingTipIds = FindBadgeIdsToAddToMobileDatabase(serverIds, mobileIds);

                    if (missingTipIds.Count > 0)
                    {
                        missingBadges = await GetMissingBadgesFromServer(missingTipIds, cancellationToken);
                    }
                }

                return new SyncBadgesResponse()
                {
                    MissingBadges = missingBadges
                };
            }

            private async Task<List<Guid>> GetServerMindfulnessTipIds(CancellationToken cancellationToken)
            {
                try
                {
                    return await _context.Badges
                        .Select(tip => tip.Id)
                        .ToListAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when gathering current tips from API and/or local db.", ex);
                }
            }

            private static List<Guid> FindBadgeIdsToAddToMobileDatabase(
                List<Guid> serverIds,
                IList<Guid> mobileIds)
            {
                List<Guid> missingBadgeIds;
                if (mobileIds.Any())
                {
                    missingBadgeIds = serverIds
                        .Except(mobileIds)
                        .ToList();
                }
                else
                {
                    missingBadgeIds = serverIds;
                }

                return missingBadgeIds;
            }

            private async Task<List<FocusCore.Models.BaseBadge>> GetMissingBadgesFromServer(
                List<Guid> missingTipIds,
                CancellationToken cancellationToken)
            {
                try
                {
                    return await _context.Badges
                        .Where(badge => missingTipIds.Contains(badge.Id))
                        .Select(tip => new FocusCore.Models.BaseBadge()
                        {
                            Id = tip.Id,
                            Image = tip.Image,
                            Name = tip.Name
                        })
                        .ToListAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when gathering current tips from API and/or local db.", ex);
                }
            }   
        }

    }
}
