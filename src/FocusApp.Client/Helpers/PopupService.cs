using CommunityToolkit.Maui.Views;

namespace FocusApp.Client.Helpers
{
    public interface IPopupService
    {
        void ShowPopup<T>() where T : Popup;
        void HidePopup();
        void HideAllPopups();
    }

    public class PopupService : IPopupService
    {
        private readonly IServiceProvider _services;

        // Need to keep track of the popups so we can close them.
        private Stack<Popup> _popups = new();

        public PopupService(IServiceProvider _services)
        {
            this._services = _services;
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
            mainPage.ShowPopup<T>(popup);
            _popups.Push(popup);
        }

        public void HidePopup()
        {
            _popups.Pop().Close();
        }

        public void HideAllPopups()
        {
            while (_popups.Count > 0)
            {
                _popups.Pop().Close();
            }
        }
    }
}
