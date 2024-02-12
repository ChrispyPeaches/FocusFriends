using CommunityToolkit.Maui.Views;

namespace FocusApp.Helpers
{
    public interface IPopupService
    {
        void ShowPopup<T>() where T : Popup;
        void HidePopup();
        void HideAllPopups();
    }

    public class PopupService : IPopupService
    {
        private readonly IServiceProvider services;

        // Need to keep track of the popups so we can close them.
        private Stack<Popup> popups = new();

        public PopupService(IServiceProvider services)
        {
            this.services = services;
        }

        /// <summary>
        /// Show popup.
        /// </summary>
        /// <typeparam name="T">Popup</typeparam>
        /// <exception cref="MissingMethodException"></exception>
        public void ShowPopup<T>() where T : Popup
        {
            var mainPage = App.Current?.MainPage ?? throw new MissingMethodException("Main page is null");
            var popup = services.GetRequiredService<T>();
            mainPage.ShowPopup<T>(popup);
            popups.Push(popup);
        }

        public void HidePopup()
        {
            popups.Pop().Close();
        }

        public void HideAllPopups()
        {
            while (popups.Count > 0)
            {
                popups.Pop().Close();
            }
        }
    }
}
