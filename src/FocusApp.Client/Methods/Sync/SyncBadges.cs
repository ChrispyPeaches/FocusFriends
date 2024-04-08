using MediatR;
using Microsoft.EntityFrameworkCore;
using FocusApp.Client.Clients;
using FocusApp.Shared.Data;
using FocusCore.Queries.Sync;
using FocusCore.Responses.Sync;

namespace FocusApp.Client.Methods.Sync;

public class SyncBadges
{
    public class Query : IRequest
    { }

    public class Handler : IRequestHandler<Query>
    {
        private readonly FocusAppContext _context;
        private readonly IAPIClient _client;

        public Handler(
            FocusAppContext context,
            IAPIClient client)
        {
            _context = context;
            _client = client;
        }

        public async Task Handle(
            Query query,
            CancellationToken cancellationToken)
        {
            List<Guid> mobileIds = await GetMobileDbBadgeIds(cancellationToken);

            var badgesToAdd = await GetMissingBadgesFromServer(mobileIds, cancellationToken);
            if (badgesToAdd.Count > 0)
            {
                await AddNewBadgesToMobileDb(badgesToAdd, cancellationToken);
            }
        }

        private async Task<List<Guid>> GetMobileDbBadgeIds(CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Badges
                    .Select(badge => badge.Id)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when fetching badges from local DB.", ex);
            }
        }
        private async Task<IList<Shared.Models.Badge>> GetMissingBadgesFromServer(
            List<Guid> mobileBadgeIds,
            CancellationToken cancellationToken)
        {
            SyncBadgesResponse response;
            try
            {
                response = await _client.SyncBadges(
                    new SyncBadgesQuery()
                    {
                        BadgeIds = mobileBadgeIds
                    }
                    , cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when fetching badges to add from the API.", ex);
            }

            return response.MissingBadges
                .Select(badge => new Shared.Models.Badge()
                {
                    Id = badge.Id,
                    Name = badge.Name,
                    Image = badge.Image
                })
                .ToList();
        }


        private async Task AddNewBadgesToMobileDb(
            IEnumerable<Shared.Models.Badge> badgesToAdd,
            CancellationToken cancellationToken)
        {
            try
            {
                await _context.Badges.AddRangeAsync(badgesToAdd, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when adding new badges to the local DB.", ex);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
