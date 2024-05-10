using FocusApp.Client.Methods.Sync;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FocusApp.Client.Helpers;

public interface ISyncService
{
    IQueryable<Island> GetInitialIslandQuery();
    IQueryable<Pet> GetInitialPetQuery();

    /// <summary>
    /// Ensure the database is created and migrations are applied, then run the startup logic.
    /// </summary>
    Task RunStartupLogic();

    /// <summary>
    /// Populates the database with initial data requested from the API for any of
    /// the island, pets, or decor tables if they don't have any entries.
    /// </summary>
    Task GatherEssentialDatabaseData();

    /// <summary>
    /// Syncs all item types in the <see cref="SyncItems.SyncItemType"/> enum between the API and mobile database
    /// if the last sync happened over a week ago or this is the first time starting the app.
    /// </summary>
    /// <remarks>
    /// If there's an unexpected error, the critical data for app functionality will be retrieved.
    /// </remarks>
    Task StartupSync();
}

public class SyncService : ISyncService
{
    private readonly FocusAppContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ISyncService> _logger;

    public SyncService(
        FocusAppContext context,
        IServiceProvider serviceProvider,
        ILogger<ISyncService> logger)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _logger = logger;
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

    /// <summary>
    /// Ensure the database is created and migrations are applied, then run the startup logic.
    /// </summary>
    public async Task RunStartupLogic()
    {
        
    }

    /// <summary>
    /// Syncs all item types in the <see cref="SyncItems.SyncItemType"/> enum between the API and mobile database
    /// if the last sync happened over a week ago or this is the first time starting the app.
    /// </summary>
    /// <remarks>
    /// If there's an unexpected error, the critical data for app functionality will be retrieved.
    /// </remarks>
    public async Task StartupSync()
    {
        try
        {

            // If not in debug and a sync has been done in the past week, don't sync
#if !DEBUG
                string lastSyncTimeString = PreferencesHelper.Get<string>(PreferencesHelper.PreferenceNames.last_sync_time);
                if (!string.IsNullOrEmpty(lastSyncTimeString))
                {
                    DateTimeOffset lastSyncTime = DateTimeOffset.Parse(lastSyncTimeString);

                    if (DateTimeOffset.UtcNow < lastSyncTime.AddDays(7))
                        return;
                }
#endif

            IList<Task> tasks = new List<Task>();
            var scopedServiceProvider = _serviceProvider
                .CreateScope()
                .ServiceProvider;
            _ = scopedServiceProvider.GetRequiredService<FocusAppContext>();

            foreach (SyncItems.SyncItemType syncType in Enum.GetValues(typeof(SyncItems.SyncItemType)))
            {
                IMediator mediator = _serviceProvider
                    .CreateScope()
                    .ServiceProvider
                    .GetRequiredService<IMediator>();

                tasks.Add(
                    Task.Run(() => mediator.Send(
                            new SyncItems.Query() { ItemType = syncType }
                            ))
                );
            }

            await Task.WhenAll(tasks);

            PreferencesHelper.Set(PreferencesHelper.PreferenceNames.last_sync_time, DateTimeOffset.UtcNow.ToString("O"));
        }
        catch (Exception ex)
        {
            // If there's an error with island or pet sync, ensure the essential database information is gathered
            _logger.LogError(ex, "Error occurred when syncing selectable items and mindfulness tips. Running essential database population.");
            await GatherEssentialDatabaseData();
        }
    }

    /// <summary>
    /// Populates the database with initial data requested from the API for any of
    /// the island, pets, or decor tables if they don't have any entries.
    /// </summary>
    public async Task GatherEssentialDatabaseData()
    {
        try
        {
            var scopedServiceProvider = _serviceProvider
                .CreateScope()
                .ServiceProvider;
            IMediator mediator = scopedServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new SyncInitialData.Query());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when running essential database population.");
        }
    }
}