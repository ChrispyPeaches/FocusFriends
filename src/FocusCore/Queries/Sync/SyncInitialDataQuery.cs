using FocusCore.Responses.Sync;
using MediatR;

namespace FocusCore.Queries.Sync;

public class SyncInitialDataQuery : IRequest<SyncInitialDataResponse>
{
    public bool SyncInitialIsland { get; set; }
    public bool SyncInitialPet { get; set; }
    public bool SyncInitialDecor { get; set; }
}

