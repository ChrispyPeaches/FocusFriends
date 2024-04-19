using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Resources;

namespace FocusApp.Client.Views;
internal class GenericLoadingPopupInterface : BasePopup
{
    public GenericLoadingPopupInterface()
    {
        CanBeDismissedByTappingOutsideOfPopup = false;
        Content = new Border
        {
            StrokeThickness = 1,
            BackgroundColor = Colors.White,
            WidthRequest = 180,
            HeightRequest = 180,
            Content = new ActivityIndicator
            {
                Color = AppStyles.Palette.OrchidPink,
                IsRunning = true
            }.Center()
        };
    }
}