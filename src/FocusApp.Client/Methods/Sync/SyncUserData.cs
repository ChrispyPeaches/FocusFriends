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
            if (query.ServerUser == null) return Unit.Value;

            _serverUser = query.ServerUser;

            _localUser = await _localContext.Users
                .Include(u => u.Pets)
                .Include(u => u.Decor)
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

            if (outOfSyncPetIds != null && outOfSyncPetIds?.Count != 0)
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

            if (outOfSyncIslandIds != null && outOfSyncIslandIds?.Count != 0)
            {
                foreach (Guid islandId in outOfSyncIslandIds)
                {
                    _localUser.Islands?.Add(new UserIsland { Island = _localContext.Islands.First(i => i.Id == islandId) });
                }
            }
        }

        void SyncUserDecor()
        {
            List<Guid>? serverDecorIds = _serverUser.Decor?.Select(ud => ud.DecorId).ToList();
            List<Guid>? localDecorIds = _localUser.Decor?.Select(ud => ud.DecorId).ToList();
            List<Guid>? outOfSyncDecorIds = serverDecorIds?.Except(localDecorIds).ToList();

            if (outOfSyncDecorIds != null && outOfSyncDecorIds?.Count != null)
            {
                foreach (Guid decorId in outOfSyncDecorIds)
                {
                    _localUser.Decor?.Add(new UserDecor { Decor = _localContext.Decor.First(f => f.Id == decorId) });
                }
            }
        }

        void SyncUserBadges()
        {
            List<Guid>? serverBadgeIds = _serverUser.Badges?.Select(ub => ub.BadgeId).ToList();
            List<Guid>? localBadgeIds = _localUser.Badges?.Select(ub => ub.BadgeId).ToList();
            List<Guid>? outOfSyncBadgeIds = serverBadgeIds?.Except(localBadgeIds).ToList();

            if (outOfSyncBadgeIds != null && outOfSyncBadgeIds?.Count != null)
            {
                foreach (Guid badgeId in outOfSyncBadgeIds)
                {
                    _localUser.Badges?.Add(new UserBadge { Badge = _localContext.Badges.First(b => b.Id == badgeId) });
                }
            }
        }
    }
}

