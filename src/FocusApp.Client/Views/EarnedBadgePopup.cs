using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Helpers;
using FocusApp.Client.Methods.Shop;
using FocusApp.Client.Resources;
using FocusApp.Client.Views.Mindfulness;
using FocusApp.Shared.Data;
using FocusCore.Extensions;
using FocusCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Shapes;
using FocusApp.Client.Extensions;
using FocusApp.Shared.Models;
using SimpleToolkit.Core;
using FocusApp.Client.Resources.FontAwesomeIcons;

namespace FocusApp.Client.Views
{
    internal class EarnedBadgePopupInterface : BadgePopup
    {
        Helpers.PopupService _popupService;
        StackLayout _popupContentStack;

        public EarnedBadgePopupInterface(PopupService popupService)
        {
            _popupService = popupService;

            // Set popup location
            HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            Color = Colors.Transparent;

            CanBeDismissedByTappingOutsideOfPopup = false;

            _popupContentStack = new StackLayout();
            /*
            new Button
            {
                Text = SolidIcons.x,
                TextColor = Colors.Black,
                FontFamily = nameof(SolidIcons),
                FontSize = 20,
                BackgroundColor = Colors.Transparent
            }
            */

            Content = new Border
            {
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20, 20, 20, 20) },
                BackgroundColor = AppStyles.Palette.LightMauve,
                WidthRequest = 360,
                HeightRequest = 460,
                Content = _popupContentStack
            };
        }

        public override void PopulatePopup(Badge badge)
        {
            // Shop item name label
            Label badgeName = new Label
            {
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
            .Margins(top: 10);
            badgeName.SetBinding(Label.TextProperty, "Name");
            badgeName.BindingContext = badge;

            // Horizontal Divider
            BoxView divider = new BoxView
            {
                Color = Color.Parse("Black"),
                WidthRequest = 400,
                HeightRequest = 2,
                Margin = 20
            }
            .Bottom()
            .Row(0)
            .Column(0)
            .ColumnSpan(2);

            // Shop item image
            Image badgeImage = new Image
            {
                WidthRequest = 150,
                HeightRequest = 150
            };
            badgeImage.SetBinding(
                Image.SourceProperty, "Image",
                converter: new ByteArrayToImageSourceConverter());
            badgeImage.BindingContext = badge;

            // Shop item price
            Label badgeDescription = new Label
            {
                FontSize = 30,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
            .Margins(top: 10);
            badgeDescription.SetBinding(Label.TextProperty, "Description");
            badgeDescription.BindingContext = badge;

            /*
            Grid popupButtons = new Grid
            {
                Children =
                {
                    new Button
                    {
                        WidthRequest = 125,
                        HeightRequest = 50,
                        Text = "Leave Me Be",
                        HorizontalOptions = LayoutOptions.Start,
                    }
                    .Margins(left: 35, top: 50)
                    .Invoke(button => button.Pressed += (s,e) => ExitItemPopup(s,e)),
                    buyButton
                }
            };
            */

            _popupContentStack.Add(badgeName);
            _popupContentStack.Add(divider);
            _popupContentStack.Add(badgeImage);
            _popupContentStack.Add(badgeDescription);
            //_popupContentStack.Add(popupButtons);
        }

        // User doesn't want to purchase the item, hide popup
        private async void ExitItemPopup(object sender, EventArgs e)
        {
            _popupService.HidePopup();
        }
    }
}