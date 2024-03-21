using MediatR;
using Microsoft.EntityFrameworkCore;
using FocusApp.Client.Clients;
using FocusApp.Shared.Data;
using FocusCore.Queries.Sync;
using FocusCore.Responses.Sync;

namespace FocusApp.Client.Methods.Sync
{
    public class SyncMindfulnessTips
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

                var tipsToAdd = await GetMissingTipsFromServer(mobileIds, cancellationToken);
                if (tipsToAdd.Count > 0)
                {
                    await AddNewTipsToMobileDb(tipsToAdd, cancellationToken);
                }
            }

            private async Task<List<Guid>> GetMobileDbMindfulnessTipIds(CancellationToken cancellationToken)
            {
                try
                {
                    return await _context.MindfulnessTips
                        .Select(tip => tip.Id)
                        .ToListAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when gathering current tips from local DB.", ex);
                }
            }

            private async Task<IList<Shared.Models.MindfulnessTip>> GetMissingTipsFromServer(
                List<Guid> mobileTipIds,
                CancellationToken cancellationToken)
            {
                SyncMindfulnessTipsResponse response;
                try
                {
                    response = await _client.SyncMindfulnessTips(
                        new SyncMindfulnessTipsQuery()
                        {
                            MindfulnessTipIds = mobileTipIds
                        }
                        , cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when gathering tips to add from the API.", ex);
                }

                return response.MissingTips
                    .Select(tip => new Shared.Models.MindfulnessTip()
                    {
                        Id = tip.Id,
                        Title = tip.Title,
                        Content = tip.Content,
                        SessionRatingLevel = tip.SessionRatingLevel
                    })
                    .ToList();

                
            }

            private async Task AddNewTipsToMobileDb(
                IEnumerable<Shared.Models.MindfulnessTip> tipsToAdd,
                CancellationToken cancellationToken)
            {
                try
                {
                    await _context.MindfulnessTips.AddRangeAsync(tipsToAdd, cancellationToken);
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
