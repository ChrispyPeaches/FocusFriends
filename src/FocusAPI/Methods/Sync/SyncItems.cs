using FocusAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FocusCore.Models;
using FocusCore.Queries.Sync;

namespace FocusAPI.Methods.Sync;

public class SyncItems
{
    public class Query<TItem> : SyncItemsQuery, IRequest<Response<TItem>> 
        where TItem : class, ISyncEntity
    { }

    public class Response<TItem>
    {
        public IList<TItem> MissingItems { get; set; }
    }

    public class Validator<TItem> : AbstractValidator<Query<TItem>>
        where TItem : class, ISyncEntity
    {
        public Validator()
        {
            RuleFor(query => query.ItemIds)
                .NotNull();
        }
    }

    public class Handler<TItem> : IRequestHandler<Query<TItem>, Response<TItem>>
        where TItem : class, ISyncEntity
    {
        private readonly FocusContext _context;

        public Handler(FocusContext context)
        {
            _context = context;
        }

        public async Task<Response<TItem>> Handle(
            Query<TItem> query,
            CancellationToken cancellationToken)
        {
            List<Guid> serverIds = await GetServerMindfulnessTipIds(cancellationToken);
            IList<Guid> mobileIds = query.ItemIds;

            List<TItem> missingItems = new();
            if (serverIds.Count != 0)
            {
                List<Guid> missingTipIds = FindItemIdsToAddToMobileDatabase(serverIds, mobileIds);

                if (missingTipIds.Count > 0)
                {
                    missingItems = await GetMissingItemsFromServer(missingTipIds, cancellationToken);
                }
            }

            return new Response<TItem>()
            {
                MissingItems = missingItems
            };
        }

        private async Task<List<Guid>> GetServerMindfulnessTipIds(CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Set<TItem>()
                    .Select(item => item.Id)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when gathering current tips from API and/or local db.", ex);
            }
        }

        private static List<Guid> FindItemIdsToAddToMobileDatabase(
            List<Guid> serverIds,
            IList<Guid> mobileIds)
        {
            List<Guid> missingItemIds;
            if (mobileIds.Any())
            {
                missingItemIds = serverIds
                    .Except(mobileIds)
                    .ToList();
            }
            else
            {
                missingItemIds = serverIds;
            }

            return missingItemIds;
        }

        private async Task<List<TItem>> GetMissingItemsFromServer(
            List<Guid> missingItemIds,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Set<TItem>()
                    .Where(item => missingItemIds.Contains(item.Id))
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when gathering current items from API and/or local db.", ex);
            }
        }   
    }

}