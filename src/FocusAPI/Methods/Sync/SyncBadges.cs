using FluentValidation;
using FocusAPI.Data;
using FocusCore.Queries.Sync;
using FocusCore.Responses.Sync;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Methods.Sync;

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
            List<Guid> serverIds = await GetServerBadgeIds(cancellationToken);
            IList<Guid> mobileIds = query.BadgeIds;

            List<FocusCore.Models.BaseBadge> missingBadges = new();
            if (serverIds.Count != 0)
            {
                List<Guid> missingBadgeIds = FindBadgeIdsToAddToMobileDatabase(serverIds, mobileIds);

                if (missingBadgeIds.Count > 0)
                {
                    missingBadges = await GetMissingBadgesFromServer(missingBadgeIds, cancellationToken);
                }
            }

            return new SyncBadgesResponse()
            {
                MissingBadges = missingBadges
            };
        }

        private async Task<List<Guid>> GetServerBadgeIds(CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Badges
                    .Select(badge => badge.Id)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when gathering current badges from API and/or local db.", ex);
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
            List<Guid> missingBadgeIds,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Badges
                    .Where(badge => missingBadgeIds.Contains(badge.Id))
                    .Select(badge => new FocusCore.Models.BaseBadge()
                    {
                        Id = badge.Id,
                        Name = badge.Name,
                        Image = badge.Image
                    })
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when gathering current badges from API and/or local db.", ex);
            }
        }
    }
}
