using FocusCore.Responses.Sync;
using MediatR;

namespace FocusCore.Queries.Sync;

public class SyncItemsQuery
{
    public IList<Guid> ItemIds { get; set; }
}