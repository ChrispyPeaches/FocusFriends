namespace FocusApp.Client.Helpers;

internal static class PreferencesHelper
{
    /// <summary> A collection of default values for the preferences stored and used by the FocusApp </summary>
    private static readonly Dictionary<string, dynamic?> DefaultValues = new()
    {
        { nameof(PreferenceNames.startup_tips_enabled), true },
        { nameof(PreferenceNames.session_rating_enabled), true },
        { nameof(PreferenceNames.last_sync_time), null as DateTimeOffset? }
    };

    /// <summary> A collection of the names for preferences stored and used by the FocusApp. </summary>
    public enum PreferenceNames
    {
        startup_tips_enabled,
        session_rating_enabled,
        last_sync_time
    }

    /// <summary>
    /// Retrieve a specific FocusApp preference, or its default value if one isn't already stored
    /// </summary>
    /// <typeparam name="TPrefType">The datatype of the preference you're retrieving</typeparam>
    public static TPrefType Get<TPrefType>(PreferenceNames prefName)
    {
        return Preferences.Default.Get<TPrefType>(prefName.ToString(), DefaultValues[prefName.ToString()]);
    }

    /// <summary>
    /// Set a specific FocusApp preference
    /// </summary>
    /// <typeparam name="TPrefType">The datatype of the preference you're setting</typeparam>
    public static void Set<TPrefType>(PreferenceNames prefName, TPrefType value)
    {
        Preferences.Default.Set(prefName.ToString(), value);
    }
}