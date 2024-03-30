using System.Reflection.Metadata;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FocusApp.Client.Clients;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Queries.Sync;
using FocusCore.Responses.Sync;
using Microsoft.Extensions.Logging;
using System.Threading;
using FocusApp.Client.Helpers;
using FocusCore.Models;

namespace FocusApp.Client.Methods.Sync
{
    public class SyncInitialData
    {
        public class Query : IRequest
        { }

        public class Handler : IRequestHandler<Query>
        {
            private readonly FocusAppContext _context;
            private readonly IAPIClient _client;
            private readonly ILogger<Handler> _logger;

            public Handler(
                FocusAppContext context,
                IAPIClient client,
                ILogger<Handler> logger)
            {
                _context = context;
                _client = client;
                _logger = logger;
            }

            public async Task Handle(
                Query query,
                CancellationToken cancellationToken = default)
            {
                try
                {
                    int islandCount = await GetCount(_context.Islands, cancellationToken);
                    int petCount = await GetCount(_context.Pets, cancellationToken);

                    if (islandCount == 0 || petCount == 0)
                    {
                        SyncInitialDataResponse response = await GetItemsFromServer(new SyncInitialDataQuery()
                            {
                                SyncInitialIsland = islandCount == 0,
                                SyncInitialPet = petCount == 0
                            },
                            cancellationToken);

                        if (response.Island is not null)
                        {
                            await AddItemToMobileDb(
                                _context.Islands,
                                ProjectionHelper.ProjectFromBaseIsland(response.Island),
                                cancellationToken);
                        }

                        if (response.Pet is not null)
                        {
                            await AddItemToMobileDb(
                                _context.Pets,
                                ProjectionHelper.ProjectFromBasePet(response.Pet),
                                cancellationToken);
                        }

                        if (_context.ChangeTracker.HasChanges())
                        {
                            await SaveChangesToMobileDb(cancellationToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error when syncing initial data.");
                }
            }

            private async Task<int> GetCount<T>(
                DbSet<T> dbSet,
                CancellationToken cancellationToken = default)
                where T : class
            {
                try
                {
                    return await dbSet.CountAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error when gathering count of {nameof(T)} from local DB.", ex);
                }
            }

            private async Task<SyncInitialDataResponse> GetItemsFromServer(
                SyncInitialDataQuery query,
                CancellationToken cancellationToken = default)
            {
                SyncInitialDataResponse response;
                try
                {
                    return await _client.SyncInitialData(query, cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when gathering items to add from the API.", ex);
                }
            }

            private async Task AddItemToMobileDb<T>(
                DbSet<T> dbSet,
                T itemToAdd,
                CancellationToken cancellationToken = default)
                where T : class
            {
                try
                {
                    await dbSet.AddAsync(itemToAdd, cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error when adding new {nameof(T)}s to the local db.", ex);
                }

                await _context.SaveChangesAsync(cancellationToken);
            }

            private async Task SaveChangesToMobileDb(CancellationToken cancellationToken = default)
            {
                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when saving changes to the local db.", ex);
                }
            }
        }

    }
}
