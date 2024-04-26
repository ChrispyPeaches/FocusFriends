using CommunityToolkit.Maui.Views;
using FocusApp.Shared.Models;

namespace FocusApp.Client.Views;

/// <summary>
/// Used for simplifying popup registration
/// </summary>
internal class BasePopup : Popup
{
    // Virtual method for populating badge popups from the popup service.
    // TODO: Update all popups to use an async override for populating information
    // so that the population can be done from the popup service
    public virtual void PopulatePopup(Badge badge) { }
}