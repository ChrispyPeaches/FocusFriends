using System.Linq.Expressions;
using FocusAPI.Data;
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

            public Handler(FocusContext context)
            {
                _context = context;
            }

            public async Task<SyncInitialDataResponse> Handle(
                SyncInitialDataQuery query,
                CancellationToken cancellationToken = default)
            {
                SyncInitialDataResponse response = new();

                if (query.SyncInitialIsland)
                {
                    response.Island = await GetInitialItem(
                        _context.Islands, 
                        island => island.Name == "Tropical",
                        cancellationToken);
                }

                if (query.SyncInitialPet)
                {
                    response.Pet = await GetInitialItem(
                        _context.Pets,
                        pet => pet.Name == "Cool Cat",
                        cancellationToken);
                }

                return response;
            }

            private async Task<T?> GetInitialItem<T>(
                DbSet<T> dbSet,
                Expression<Func<T, bool>>? filterExpression = null,
                CancellationToken cancellationToken = default)
                where T : class
            {
                try
                {
                    IQueryable<T> query = dbSet;

                    if (filterExpression != null)
                    {
                        query = query.Where(filterExpression);
                    }

                    return await query.FirstOrDefaultAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when gathering current tips from API and/or local db.", ex);
                }
            }
        }

    }
}
