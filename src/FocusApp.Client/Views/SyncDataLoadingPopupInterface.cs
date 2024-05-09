using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Resources;

namespace FocusApp.Client.Views;
internal class SyncDataLoadingPopupInterface : BasePopup
{
    public SyncDataLoadingPopupInterface()
    {
        CanBeDismissedByTappingOutsideOfPopup = false;
        Content = new Border
        {
            StrokeThickness = 1,
            BackgroundColor = Colors.White,
            WidthRequest = 180,
            HeightRequest = 200,

            Content = new Grid()
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                    40,
                    GridRowsColumns.Star
                ),
                Children =
                {
                    new Label()
                    {
                        Text = "Downloading Content",
                        FontSize = 16,
                        TextColor = Colors.Black
                    }
                    .Center()
                    .Row(0)
                    ,
                    new ActivityIndicator
                    {
                        Color = AppStyles.Palette.OrchidPink,
                        IsRunning = true
                    }
                    .Center()
                    .Row(1)
                }
                
            }
        };
    }
}