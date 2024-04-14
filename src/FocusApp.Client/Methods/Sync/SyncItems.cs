using MediatR;
using Microsoft.EntityFrameworkCore;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Models;

namespace FocusApp.Client.Methods.Sync;

public class SyncItems
{
    public class Query : IRequest
    {
        public SyncItemType ItemType { get; set; }
    }
    public enum SyncItemType
    {
        MindfulnessTips,
        Badges,
        Pets,
        Decor,
        Islands
    }

    public class Handler : IRequestHandler<Query>
    {
        private readonly FocusAppContext _context;
        private readonly IAPIClient _client;

        public Handler(FocusAppContext context, IAPIClient client)
        {
            _context = context;
            _client = client;
        }

        public async Task Handle(
            Query query,
            CancellationToken cancellationToken)
        {
            List<Guid> mobileIds;
            switch (query.ItemType)
            {
                case SyncItemType.MindfulnessTips:
                    mobileIds = await GetMobileDbMindfulnessTipIds<Shared.Models.MindfulnessTip>(cancellationToken);
                    var tipsToAdd = await GetMissingTipsFromServer(mobileIds, cancellationToken);
                    if (tipsToAdd.Count > 0)
                        await AddNewItemsToMobileDb(tipsToAdd, cancellationToken);
                    break;
                case SyncItemType.Badges:
                    mobileIds = await GetMobileDbMindfulnessTipIds<Badge>(cancellationToken);
                    var badgesToAdd = await GetMissingBadgesFromServer(mobileIds, cancellationToken);
                    if (badgesToAdd.Count > 0)
                        await AddNewItemsToMobileDb(badgesToAdd, cancellationToken);
                    break;
                case SyncItemType.Pets:
                    mobileIds = await GetMobileDbMindfulnessTipIds<Pet>(cancellationToken);
                    var petsToAdd = await GetMissingPetsFromServer(mobileIds, cancellationToken);
                    if (petsToAdd.Count > 0)
                        await AddNewItemsToMobileDb(petsToAdd, cancellationToken);
                    break;
                case SyncItemType.Decor:
                    mobileIds = await GetMobileDbMindfulnessTipIds<Pet>(cancellationToken);
                    var decorToAdd = await GetMissingDecorFromServer(mobileIds, cancellationToken);
                    if (decorToAdd.Count > 0)
                        await AddNewItemsToMobileDb(decorToAdd, cancellationToken);
                    break;
                case SyncItemType.Islands:
                    mobileIds = await GetMobileDbMindfulnessTipIds<Pet>(cancellationToken);
                    var islandsToAdd = await GetMissingIslandsFromServer(mobileIds, cancellationToken);
                    if (islandsToAdd.Count > 0)
                        await AddNewItemsToMobileDb(islandsToAdd, cancellationToken);
                    break;
            }
        }

        private async Task<List<Guid>> GetMobileDbMindfulnessTipIds<TItem>(CancellationToken cancellationToken)
            where TItem : class, ISyncEntity
        {
            try
            {
                return await _context.Set<TItem>()
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
            try
            {
                var response = await _client.SyncMindfulnessTips(
                        new()
                        {
                            ItemIds = mobileTipIds
                        },
                        cancellationToken);

                return response
                    .MissingItems
                    .Select(ProjectionHelper.ProjectFromBaseMindfulnessTip)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when gathering tips to add from the API.", ex);
            }
        }

        private async Task<IList<Badge>> GetMissingBadgesFromServer(
            List<Guid> mobileTipIds,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _client.SyncBadges(
                    new()
                    {
                        ItemIds = mobileTipIds
                    },
                    cancellationToken);

                return response
                    .MissingItems
                    .Select(ProjectionHelper.ProjectFromBaseBadge)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when gathering badges to add from the API.", ex);
            }
        }

        private async Task<IList<Pet>> GetMissingPetsFromServer(
            List<Guid> mobileTipIds,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _client.SyncPets(
                    new()
                    {
                        ItemIds = mobileTipIds
                    },
                    cancellationToken);

                return response
                    .MissingItems
                    .Select(ProjectionHelper.ProjectFromBasePet)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when gathering pets to add from the API.", ex);
            }
        }

        private async Task<IList<Furniture>> GetMissingDecorFromServer(
            List<Guid> mobileTipIds,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _client.SyncDecor(
                    new()
                    {
                        ItemIds = mobileTipIds
                    },
                    cancellationToken);

                return response
                    .MissingItems
                    .Select(ProjectionHelper.ProjectFromBaseFurniture)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when gathering decor to add from the API.", ex);
            }
        }

        private async Task<IList<Island>> GetMissingIslandsFromServer(
            List<Guid> mobileTipIds,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _client.SyncIslands(
                    new()
                    {
                        ItemIds = mobileTipIds
                    },
                    cancellationToken);

                return response
                    .MissingItems
                    .Select(ProjectionHelper.ProjectFromBaseIsland)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when gathering islands to add from the API.", ex);
            }
        }

        private async Task AddNewItemsToMobileDb<TItem>(
            IEnumerable<TItem> tipsToAdd,
            CancellationToken cancellationToken)
            where TItem : class, ISyncEntity
        {
            try
            {
                await _context.Set<TItem>().AddRangeAsync(tipsToAdd, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when adding new tips to the local db.", ex);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}