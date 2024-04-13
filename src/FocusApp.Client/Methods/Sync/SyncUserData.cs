using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Client.Methods.Sync;
public class SyncUserData
{
    public class Query : IRequest<Unit> 
    {
        public Shared.Models.User ServerUser { get; set; }
    }

    public class Handler : IRequestHandler<Query, Unit>
    {
        FocusAppContext _localContext;
        Shared.Models.User _serverUser;
        Shared.Models.User _localUser;
        public Handler(FocusAppContext localContext) 
        {
            _localContext = localContext;
        }

        public async Task<Unit> Handle(Query query, CancellationToken cancellationToken) 
        {
            if (query.ServerUser == null) return;

            _serverUser = query.ServerUser;

            _localUser = await _localContext.Users
                .Include(u => u.Pets)
                .Include(u => u.Furniture)
                .Include(u => u.Islands)
                .FirstOrDefaultAsync(u => u.Id == _serverUser.Id);

            SyncUserPets();
            SyncUserIslands();
            SyncUserDecor();
            SyncUserBadges();

            await _localContext.SaveChangesAsync();

            return Unit.Value;
        }

        void SyncUserPets()
        {
            List<Guid>? serverPetIds = _serverUser.Pets?.Select(up => up.PetId).ToList();
            List<Guid>? localPetIds = _localUser.Pets?.Select(up => up.PetId).ToList();
            List<Guid>? outOfSyncPetIds = serverPetIds?.Except(localPetIds).ToList();

            if (outOfSyncPetIds != null && outOfSyncPetIds.Any())
            {
                foreach (var petId in outOfSyncPetIds)
                {
                    _localUser.Pets?.Add(new UserPet { Pet = _localContext.Pets.First(p => p.Id == petId) });
                }
            }
        }

        void SyncUserIslands()
        {
            List<Guid>? serverIslandIds = _serverUser.Islands?.Select(ui => ui.IslandId).ToList();
            List<Guid>? localIslandIds = _localUser.Islands?.Select(ui => ui.IslandId).ToList();
            List<Guid>? outOfSyncIslandIds = serverIslandIds?.Except(localIslandIds).ToList();

            if (outOfSyncIslandIds != null && outOfSyncIslandIds.Any())
            {
                foreach (Guid islandId in outOfSyncIslandIds)
                {
                    _localUser.Islands?.Add(new UserIsland { Island = _localContext.Islands.First(i => i.Id == islandId) });
                }
            }
        }

        void SyncUserDecor()
        {
            List<Guid>? serverFurnitureIds = _serverUser.Furniture?.Select(uf => uf.FurnitureId).ToList();
            List<Guid>? localFurnitureIds = _localUser.Furniture?.Select(uf => uf.FurnitureId).ToList();
            List<Guid>? outOfSyncFurnitureIds = serverFurnitureIds?.Except(localFurnitureIds).ToList();

            if (outOfSyncFurnitureIds != null && outOfSyncFurnitureIds.Any())
            {
                foreach (Guid furnitureId in outOfSyncFurnitureIds)
                {
                    _localUser.Furniture?.Add(new UserFurniture { Furniture = _localContext.Furniture.First(f => f.Id == furnitureId) });
                }
            }
        }

        void SyncUserBadges()
        {
            List<Guid>? serverBadgeIds = _serverUser.Badges?.Select(ub => ub.BadgeId).ToList();
            List<Guid>? localBadgeIds = _localUser.Badges?.Select(ub => ub.BadgeId).ToList();
            List<Guid>? outOfSyncBadgeIds = serverBadgeIds?.Except(localBadgeIds).ToList();

            if (outOfSyncBadgeIds != null && outOfSyncBadgeIds.Any())
            {
                foreach (Guid badgeId in outOfSyncBadgeIds)
                {
                    _localUser.Badges?.Add(new UserBadge { Badge = _localContext.Badges.First(b => b.Id == badgeId) });
                }
            }
        }
    }
}

