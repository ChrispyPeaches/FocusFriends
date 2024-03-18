using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Methods.MindfulnessTip;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Shared.Models;
using FocusCore.Extensions;
using MediatR;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.LifecycleEvents;

namespace FocusApp.Client.Views.Shop
{

    internal class MindfulnessTipPopupInterface : BasePopup
    {
        private Helpers.PopupService _popupService;
        Grid _popupContent;
        private readonly IMediator _mediator;

        enum Row { TopBar, TipDisplay}

        public MindfulnessTipPopupInterface(Helpers.PopupService popupService, IMediator mediator)
        {
            _popupService = popupService;
            _mediator = mediator;

            // Set popup location
            HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            Color = Colors.Transparent;

            _popupContent = new Grid()
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                    (Row.TopBar, 50),
                    (Row.TipDisplay, GridRowsColumns.Stars(1))
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
                        // When clicked, close the popup
                        .Invoke(button => button.Released += OnDismissPopup),
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

        public async Task PopulatePopup(
            MindfulnessTipExtensions.FocusSessionRating sessionRating,
            CancellationToken cancellationToken)
        {
            // Display loading icon

            // Get tip
            MindfulnessTip? tip = null;
            try
            {
                tip = await _mediator.Send(
                    new GetMindfulnessTipByRatingLevel.Query() { RatingLevel = sessionRating },
                    cancellationToken);
            }
            catch (Exception e)
            {
                // Handle exception
            }

            // Stop displaying loading icon


            // Display tip
            if (tip != null)
            {
                _popupContent.Children.Add(
                    new WebView()
                        {
                            Source = new HtmlWebViewSource() { Html = tip.Content }
                        }
                        .Row(Row.TipDisplay));
            }
            else
            {
                _popupService.HidePopup();
            }
        }

        // Navigate to page according to button
        private async void OnDismissPopup(object? sender, EventArgs e)
        {
            _popupService.HidePopup();
        }
    }
}
