using FocusCore.Responses.Sync;
using MediatR;

namespace FocusCore.Queries.Sync;

public class SyncMindfulnessTipsQuery : IRequest<SyncMindfulnessTipsResponse>
{
    public IList<Guid> MindfulnessTipIds { get; set; }
}

