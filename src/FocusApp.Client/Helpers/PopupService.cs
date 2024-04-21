using CommunityToolkit.Maui.Views;
using FocusApp.Client.Views;
using FocusApp.Client.Views.Shop;
using FocusApp.Shared.Models;

namespace FocusApp.Client.Helpers
{
    internal interface IPopupService
    {
        void ShowPopup<T>() where T : Popup;
        Popup ShowAndGetPopup<T>() where T : Popup;
        void HidePopup(bool wasDismissedByTappingOutsideOfPopup);
        void HideAllPopups();
    }

    internal class PopupService : IPopupService
    {
        private readonly IServiceProvider _services;
        private Stack<Popup> _popups;

        public PopupService(IServiceProvider _services)
        {
            this._services = _services;
            // Need to keep track of the popups so we can close them.
            _popups = new();
        }

        /// <summary>
        /// Show popup.
        /// </summary>
        /// <typeparam name="T">Popup</typeparam>
        /// <exception cref="MissingMethodException"></exception>
        public void ShowPopup<T>() where T : Popup
        {
            var mainPage = App.Current?.MainPage ?? throw new MissingMethodException("Main page is null");
            var popup = _services.GetRequiredService<T>();
            MainThread.BeginInvokeOnMainThread(() => mainPage.ShowPopup<T>(popup));
            _popups.Push(popup);
        }

        /// <summary>
        /// Show popup and return the object for populating with shop data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="MissingMethodException"></exception>
        public Popup ShowAndGetPopup<T>() where T : Popup
        {
            var mainPage = App.Current?.MainPage ?? throw new MissingMethodException("Main page is null");
            var popup = _services.GetRequiredService<T>();
            MainThread.BeginInvokeOnMainThread(() => mainPage.ShowPopup<T>(popup));
            _popups.Push(popup);
            return popup;
        }

        public void TriggerBadgeEvent<T>(Badge badge) where T : BasePopup
        {
            Thread.Sleep(1000);
            BasePopup popup = (T)ShowAndGetPopup<T>();
            popup.PopulatePopup(badge);
        }

        public void HidePopup(bool wasDismissedByTappingOutsideOfPopup = false)
        {
            if (_popups.Count > 0 && !wasDismissedByTappingOutsideOfPopup)
            {
                MainThread.BeginInvokeOnMainThread(() => _popups.Pop().Close());
            }
            // Quick patch for popups closed via tapping outside of the popup
            else if (_popups.Count > 0 && wasDismissedByTappingOutsideOfPopup)
            {
                // Remove the popup from the stack, but do not close it, as it has already
                // been closed/disposed via tapping outside of popup
                _popups.Pop();
            }
        }

        public void HideAllPopups()
        {
            while (_popups.Count > 0)
            {
                MainThread.BeginInvokeOnMainThread(() => _popups.Pop().Close());
            }
        }
    }
}
