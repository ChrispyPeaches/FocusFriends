#if DEBUG
[assembly: System.Reflection.Metadata.MetadataUpdateHandlerAttribute(typeof(FocusApp.Helpers.HotReloadService))]
namespace FocusApp.Helpers;

/// <summary>
/// Allows C# Hot Reload to work with Community Toolkit Markup
/// </summary>
/// <remarks>
/// From https://github.com/jamesmontemagno/ThreadsApp
/// </remarks>
public static class HotReloadService
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public static event Action<Type[]?>? UpdateApplicationEvent;
    public static event Action<Type[]?>? ClearCacheEvent;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    internal static void ClearCache(Type[]? types)
    {
        if (types != null)
        {
            if ((types.Length > 0) && (types[0] != null))
            {
                ClearCacheEvent?.Invoke(types);
            }
        }
    }
    internal static void UpdateApplication(Type[]? types)
    {
        UpdateApplicationEvent?.Invoke(types);
    }
}
#endif