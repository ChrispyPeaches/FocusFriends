namespace FocusApp.Client.Helpers;

internal static class PreferencesHelper
{
    private static readonly Dictionary<string, dynamic?> _initialValues = new()
    {
        { nameof(PreferenceNames.ambiance_volume), 50.00 },
        { nameof(PreferenceNames.notifications_enabled), false },
        { nameof(PreferenceNames.startup_tips_enabled), true },
        { nameof(PreferenceNames.session_rating_enabled), true },
        { nameof(PreferenceNames.last_sync_time), null as DateTimeOffset? }
    };

    public enum PreferenceNames
    {
        ambiance_volume,
        notifications_enabled,
        startup_tips_enabled,
        session_rating_enabled,
        last_sync_time
    }

    public static TPrefType Get<TPrefType>(PreferenceNames prefName)
    {
        return Preferences.Default.Get<TPrefType>(nameof(prefName), _initialValues[nameof(prefName)]);
    }

    public static void Set<TPrefType>(PreferenceNames prefName, TPrefType value)
    {
        Preferences.Default.Set(nameof(prefName), value);
    }

    public static void InitializePreferences()
    {
        foreach (var (key, value) in _initialValues)
        {
            if (!Preferences.Default.ContainsKey(key))
            {
                Preferences.Default.Set(key, value);
            }
        }
    }
}