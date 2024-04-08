using MediatR;
using Microsoft.EntityFrameworkCore;
using FocusApp.Client.Clients;
using FocusApp.Shared.Data;
using FocusCore.Queries.Sync;
using FocusCore.Responses.Sync;

namespace FocusApp.Client.Methods.Sync
{
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
                List<Guid> mobileIds = await GetMobileDbMindfulnessTipIds(cancellationToken);

                var badgesToAdd = await GetMissingTipsFromServer(mobileIds, cancellationToken);
                if (badgesToAdd.Count > 0)
                {
                    await AddNewTipsToMobileDb(badgesToAdd, cancellationToken);
                }
            }

            private async Task<List<Guid>> GetMobileDbMindfulnessTipIds(CancellationToken cancellationToken)
            {
                try
                {
                    return await _context.Badges
                        .Select(tip => tip.Id)
                        .ToListAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when gathering current tips from local DB.", ex);
                }
            }

            private async Task<IList<Shared.Models.Badge>> GetMissingTipsFromServer(
                List<Guid> mobileTipIds,
                CancellationToken cancellationToken)
            {
                SyncBadgesResponse response;
                try
                {
                    response = await _client.SyncBadges(
                        new SyncBadgesQuery()
                        {
                            BadgeIds = mobileTipIds
                        }
                        , cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when gathering tips to add from the API.", ex);
                }

                return response.MissingBadges
                    .Select(tip => new Shared.Models.Badge()
                    {
                        Id = tip.Id,
                        Image = tip.Image,
                        Name = tip.Name
                    })
                    .ToList();
            }

            private async Task AddNewTipsToMobileDb(
                IEnumerable<Shared.Models.Badge> tipsToAdd,
                CancellationToken cancellationToken)
            {
                try
                {
                    await _context.Badges.AddRangeAsync(tipsToAdd, cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when adding new tips to the local db.", ex);
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

    }
}
