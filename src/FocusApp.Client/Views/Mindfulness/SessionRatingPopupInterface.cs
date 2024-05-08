using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Views.Settings;
using MediatR;
using Microsoft.Maui.Controls.Shapes;
using static FocusCore.Extensions.MindfulnessTipExtensions;

namespace FocusApp.Client.Views.Mindfulness;

internal class SessionRatingPopupInterface : BasePopup
{
    private Helpers.PopupService _popupService;
    private readonly Grid _popupContent;

    enum Row { TopBar, Message, RatingButtons }
    enum Column { Left, Middle, Right }

    public SessionRatingPopupInterface(Helpers.PopupService popupService)
    {
        _popupService = popupService;

        // Set popup location
        HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
        VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
        Color = Colors.Transparent;

        CanBeDismissedByTappingOutsideOfPopup = false;

        _popupContent = new Grid()
        {
            RowDefinitions = GridRowsColumns.Rows.Define(
                (Row.TopBar, 50),
                (Row.Message, GridRowsColumns.Stars(1)),
                (Row.RatingButtons, GridRowsColumns.Stars(1))
                ),
            ColumnDefinitions = GridRowsColumns.Columns.Define(
                (Column.Left, GridRowsColumns.Stars(1)),
                (Column.Middle, GridRowsColumns.Stars(1)),
                (Column.Right, GridRowsColumns.Stars(1))
                ),

            Children =
            {
                // Dismiss Popup Button
                new Button
                    {
                        Text = SolidIcons.x,
                        TextColor = Colors.Black,
                        FontFamily = nameof(SolidIcons),
                        FontSize = 20,
                        BackgroundColor = Colors.Transparent
                    }
                    .Right()
                    .CenterVertical()
                    .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                    .Row(Row.TopBar)
                    .ColumnSpan(typeof(Column).GetEnumNames().Length)
                    // When clicked, close the popup
                    .Invoke(button => button.Released += OnDismissPopup),

                // Message
                new Label()
                {
                    TextColor = Colors.Black,
                    Text = "How was your session?",
                    FontSize = 30,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                }
                .Margins(top: 10)
                .Row(Row.Message)
                .ColumnSpan(typeof(Column).GetEnumNames().Length),

                // Bad Session Rating
                new Button
                {
                    
                    Text = LineArtIcons.FaceFrown,
                    TextColor = Colors.PaleVioletRed,
                    FontFamily = nameof(LineArtIcons),
                    FontSize = 60,
                    BackgroundColor = Colors.Transparent
                }
                .CenterHorizontal()
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .Row(Row.RatingButtons)
                .Column(Column.Left)
                // When clicked, close the popup
                .Invoke(button => button.Released += (_,_) => OnSessionRating(FocusSessionRating.Bad)),

                // Fine Session Rating
                new Button
                {
                    Text = LineArtIcons.FaceMeh,
                    TextColor = Colors.Black,
                    FontFamily = nameof(LineArtIcons),
                    FontSize = 60,
                    BackgroundColor = Colors.Transparent
                }
                .CenterHorizontal()
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .Row(Row.RatingButtons)
                .Column(Column.Middle)
                // When clicked, close the popup
                .Invoke(button => button.Released += (_,_) => OnSessionRating(FocusSessionRating.Fine)),

                // Good Session Rating
                new Button
                {
                    Text = LineArtIcons.FaceSmile,
                    TextColor = Colors.PaleGreen,
                    FontFamily = nameof(LineArtIcons),
                    FontSize = 60,
                    BackgroundColor = Colors.Transparent
                }
                .CenterHorizontal()
                .CenterVertical()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .Row(Row.RatingButtons)
                .Column(Column.Right)
                // When clicked, close the popup
                .Invoke(button => button.Released += (_,_) => OnSessionRating(FocusSessionRating.Good)),
            }
        };

        Content = new Border
        {
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20, 20, 20, 20) },
            BackgroundColor = Colors.White,
            WidthRequest = 360,
            HeightRequest = 460,
            Content = _popupContent
        };
    }

    /// <summary>
    /// Show the mindfulness tip popup
    /// </summary>
    private async void OnSessionRating(FocusSessionRating rating)
    {
        _popupService.HidePopup();
        MindfulnessTipPopupInterface tipPopup = (MindfulnessTipPopupInterface)_popupService.ShowAndGetPopup<MindfulnessTipPopupInterface>();
        await Task.Run(() => tipPopup.PopulatePopup(rating, default));
    }

    // Navigate to page according to button
    private async void OnDismissPopup(object? sender, EventArgs e)
    {
        _popupService.HidePopup();
    }
}

