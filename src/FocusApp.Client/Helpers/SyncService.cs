using FocusApp.Shared.Data;
using FocusApp.Shared.Models;

namespace FocusApp.Client.Helpers;

public interface ISyncService
{
    IQueryable<Island> GetInitialIslandQuery();
    IQueryable<Pet> GetInitialPetQuery();
}

public class SyncService : ISyncService
{
    private readonly FocusAppContext _context;

    public SyncService(FocusAppContext context)
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