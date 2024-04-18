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
        private readonly FocusAPIContext _apiContext;

        public SyncService(FocusAPIContext apiContext)
        {
            _apiContext = apiContext;
        }

        public IQueryable<Island> GetInitialIslandQuery()
        {
            return _apiContext.Islands
                .Where(island => island.Name == FocusCore.Consts.NameOfInitialIsland);
        }

        public IQueryable<Pet> GetInitialPetQuery()
        {
            return _apiContext.Pets
                .Where(pet => pet.Name == FocusCore.Consts.NameOfInitialPet);
        }
    }
}
