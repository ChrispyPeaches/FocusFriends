using FocusCore.Responses.Sync;
using MediatR;

namespace FocusCore.Queries.Sync;

public class SyncBadgesQuery : IRequest<SyncBadgesResponse>
{
    public IList<Guid> BadgeIds { get; set; }
}
