using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace FocusApp.Client.Views.Settings;

internal class SettingsAboutPopupInterface : BasePopup
{
    private Helpers.PopupService _popupService;

    enum Row { TopBar, Body }

    public SettingsAboutPopupInterface(Helpers.PopupService popupService)
    {
        _popupService = popupService;

        // Set popup location
        HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
        VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;

        Color = Colors.Transparent;

        CanBeDismissedByTappingOutsideOfPopup = false;

        Content = new Border
        {
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20, 20, 20, 20) },
            BackgroundColor = AppStyles.Palette.LightMauve,
            WidthRequest = 360,
            HeightRequest = 300,
            Content = new Grid()
            {
                RowDefinitions = GridRowsColumns.Rows.Define(
                        (Row.TopBar, 50),
                        (Row.Body, GridRowsColumns.Stars(1))
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
                            .ZIndex(1)
                            .Right()
                            .CenterVertical()
                            .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                            .Row(Row.TopBar)
                            // When clicked, close the popup
                            .Invoke(button => button.Released += OnDismissPopup),

                        // Body
                        new FlexLayout()
                        {
                            AlignItems = FlexAlignItems.Start,
                            JustifyContent = FlexJustify.Start,
                            Direction = FlexDirection.Column,
                            Children =
                            {
                                new Label()
                                {
                                    Text = "Contact us",
                                    FontSize = 20,
                                    TextColor = Colors.Black
                                }.Margins(bottom: 2),
                                new Label()
                                {
                                    Text = "zenpxldev@gmail.com",
                                    TextColor = Colors.Black,
                                    FontSize = 17
                                },
                            }
                        }
                        .Row(Row.Body)
                    }
            }
        }
            .Padding(horizontalSize: 10, verticalSize: 0);
    }

    // Navigate to page according to button
    private async void OnDismissPopup(object? sender, EventArgs e)
    {
        _popupService.HidePopup();
    }
}