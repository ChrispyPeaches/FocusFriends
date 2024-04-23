using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Client.Methods.Sync;
public class SyncUserData
{
    public class Query : IRequest<Unit> 
    {
        public Shared.Models.User? ServerUser { get; set; }
        public List<Guid>? UserIslandIds { get; set; }
        public List<Guid>? UserPetIds { get; set; }
        public List<Guid>? UserDecorIds { get; set; }
        public List<Guid>? UserBadgeIds { get; set; }
    }

    public class Handler : IRequestHandler<Query, Unit>
    {
        FocusAppContext _localContext;
        Shared.Models.User? _serverUser;
        Shared.Models.User? _localUser;
        public Handler(FocusAppContext localContext) 
        {
            _localContext = localContext;
        }

        public async Task<Unit> Handle(Query query, CancellationToken cancellationToken) 
        {
            if (query.ServerUser == null) return Unit.Value;

            _serverUser = query.ServerUser;

            _localUser = await _localContext.Users
                .Include(u => u.Pets)
                .Include(u => u.Decor)
                .Include(u => u.Islands)
                .FirstOrDefaultAsync(u => u.Id == _serverUser.Id, cancellationToken);

            await SyncUserPets(query, cancellationToken);
            await SyncUserIslands(query, cancellationToken);
            await SyncUserDecor(query, cancellationToken);
            await SyncUserBadges(query, cancellationToken);

            await _localContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        async Task SyncUserPets(Query query, CancellationToken cancellationToken)
        {
            List<Guid>? serverPetIds = query.UserPetIds;
            List<Guid>? localPetIds = _localUser?.Pets?.Select(up => up.PetId).ToList();
            List<Guid>? outOfSyncPetIds = serverPetIds?.Except(localPetIds).ToList();

            if (outOfSyncPetIds != null && outOfSyncPetIds?.Count != 0)
            {
                foreach (var petId in outOfSyncPetIds)
                {
                    _localUser.Pets?.Add(new UserPet { Pet = await _localContext.Pets.FirstAsync(p => p.Id == petId, cancellationToken) });
                }
            }
        }

        async Task SyncUserIslands(Query query, CancellationToken cancellationToken)
        {
            List<Guid>? serverIslandIds = query.UserIslandIds;
            List<Guid>? localIslandIds = _localUser.Islands?.Select(ui => ui.IslandId).ToList();
            List<Guid>? outOfSyncIslandIds = serverIslandIds?.Except(localIslandIds).ToList();

            if (outOfSyncIslandIds != null && outOfSyncIslandIds?.Count != 0)
            {
                foreach (Guid islandId in outOfSyncIslandIds)
                {
                    _localUser.Islands?.Add(new UserIsland { Island = await _localContext.Islands.FirstAsync(i => i.Id == islandId, cancellationToken) });
                }
            }
        }

        async Task SyncUserDecor(Query query, CancellationToken cancellationToken)
        {
            List<Guid>? serverDecorIds = query.UserDecorIds;
            List<Guid>? localDecorIds = _localUser.Decor?.Select(ud => ud.DecorId).ToList();
            List<Guid>? outOfSyncDecorIds = serverDecorIds?.Except(localDecorIds).ToList();

            if (outOfSyncDecorIds != null && outOfSyncDecorIds?.Count != 0)
            {
                foreach (Guid decorId in outOfSyncDecorIds)
                {
                    _localUser.Decor?.Add(new UserDecor { Decor = await _localContext.Decor.FirstAsync(f => f.Id == decorId, cancellationToken) });
                }
            }
        }

        async Task SyncUserBadges(Query query, CancellationToken cancellationToken)
        {
            List<Guid>? serverBadgeIds = query.UserBadgeIds;
            List<Guid>? localBadgeIds = _localUser.Badges?.Select(ub => ub.BadgeId).ToList();
            List<Guid>? outOfSyncBadgeIds = serverBadgeIds?.Except(localBadgeIds).ToList();

            if (outOfSyncBadgeIds != null && outOfSyncBadgeIds?.Count != 0)
            {
                foreach (Guid badgeId in outOfSyncBadgeIds)
                {
                    _localUser.Badges?.Add(new UserBadge { Badge = await _localContext.Badges.FirstAsync(b => b.Id == badgeId, cancellationToken) });
                }
            }
        }
    }
}