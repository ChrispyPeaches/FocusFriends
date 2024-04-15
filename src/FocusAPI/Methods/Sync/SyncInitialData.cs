using System.Linq.Expressions;
using FocusAPI.Data;
using FocusAPI.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FocusCore.Queries.Sync;
using FocusCore.Responses.Sync;

namespace FocusAPI.Methods.Sync
{
    public class SyncInitialData
    {
        public class Handler : IRequestHandler<SyncInitialDataQuery, SyncInitialDataResponse>
        {
            private readonly FocusContext _context;
            private readonly ISyncService _syncService;

            public Handler(FocusContext context, ISyncService syncService)
            {
                _context = context;
                _syncService = syncService;
            }

            public async Task<SyncInitialDataResponse> Handle(
                SyncInitialDataQuery query,
                CancellationToken cancellationToken = default)
            {
                SyncInitialDataResponse response = new();

                if (query.SyncInitialIsland)
                {
                    response.Island = await _syncService.GetInitialIslandQuery()
                        .FirstOrDefaultAsync(cancellationToken);
                }

                if (query.SyncInitialPet)
                {
                    response.Pet = await _syncService.GetInitialPetQuery()
                        .FirstOrDefaultAsync(cancellationToken);
                }

                if (query.SyncInitialDecor)
                {
                    response.Decor = await _context.Decor
                        .FirstOrDefaultAsync(cancellationToken);
                }

                return response;
            }
        }
    }
}
