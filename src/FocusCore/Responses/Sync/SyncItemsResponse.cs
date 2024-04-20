namespace FocusCore.Responses.Sync;

public class SyncItemResponse<TBaseItem>
{
    public IList<TBaseItem> MissingItems { get; set; }
}
