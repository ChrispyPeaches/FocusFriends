using FocusAPI.Data;
using FocusAPI.Models;

namespace FocusAPI.Helpers
{
    public interface ISyncService
    {
        IQueryable<Island> GetInitialIslandQuery();
        IQueryable<Pet> GetInitialPetQuery();
    }

    public class SyncService : ISyncService
    {
        private readonly FocusContext _context;

        public SyncService(FocusContext context)
        {
            _context = context;
        }

        public IQueryable<Island> GetInitialIslandQuery()
        {
            return _context.Islands
                .Where(island => island.Name == FocusCore.Consts.NameOfInitialIsland);
        }

        public IQueryable<Pet> GetInitialPetQuery()
        {
            return _context.Pets
                .Where(pet => pet.Name == FocusCore.Consts.NameOfInitialPet);
        }
    }
}
