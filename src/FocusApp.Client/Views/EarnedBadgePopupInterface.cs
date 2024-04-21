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
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Views.Social;
using SimpleToolkit.SimpleShell.Extensions;

namespace FocusApp.Client.Views
{
    internal class EarnedBadgePopupInterface : BasePopup
    {
        Helpers.PopupService _popupService;
        StackLayout _popupContentStack;

        // Row / Column structure for badge popup
        enum PopupRow { Name, Image, Description, NavButton }
        enum PopupColumn { Center }
        Label _badgeName { get; set; }
        Image _badgeImage { get; set; }
        Label _badgeDescription { get; set; }

        public EarnedBadgePopupInterface(PopupService popupService)
        {
            _popupService = popupService;

            // Set popup location
            HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            Color = Colors.Transparent;

            CanBeDismissedByTappingOutsideOfPopup = false;

            _popupContentStack = new StackLayout();

            SetupUIElements();

            Content = new Border
            {
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20, 20, 20, 20) },
                BackgroundColor = AppStyles.Palette.LightMauve,
                WidthRequest = 360,
                HeightRequest = 460,
                Content = new Grid
                {
                    RowDefinitions = Rows.Define(
                        (PopupRow.Name, Stars(0.35)),
                        (PopupRow.Image, Stars(1)),
                        (PopupRow.Description, Stars(0.5)),
                        (PopupRow.NavButton, Stars(0.35))
                        ),
                    ColumnDefinitions = Columns.Define(
                        (PopupColumn.Center, Stars(1))
                        ),
                    Children =
                    {
                        _badgeName
                        .Row(PopupRow.Name)
                        .Column(PopupColumn.Center)
                        .Center(),

                        new Button
                        {
                            Text = SolidIcons.x,
                            TextColor = Colors.Black,
                            FontFamily = nameof(SolidIcons),
                            FontSize = 20,
                            BackgroundColor = Colors.Transparent
                        }
                        .Row(PopupRow.Name)
                        .Right()
                        .Top()
                        .Invoke(button => button.Pressed += (s,e) => OnDismissPopup(s,e)),

                        new BoxView
                        {
                            Color = Colors.Black,
                            HeightRequest = 2,
                        }
                        .Row(PopupRow.Name)
                        .Column(PopupColumn.Center)
                        .Bottom(),

                        _badgeImage
                        .Row(PopupRow.Image)
                        .Column(PopupColumn.Center)
                        .Center(),

                        _badgeDescription
                        .Row(PopupRow.Description)
                        .Column(PopupColumn.Center)
                        .Center(),

                        new Label
                        {
                            Text = "Navigate to the badges page to equip your badge!",
                            Opacity = 0.5
                        }
                        .Row(PopupRow.NavButton)
                        .Column(PopupColumn.Center)
                        .Center()
                    }
                }
            };
        }

        public override void PopulatePopup(Badge badge)
        {
            _badgeName.BindingContext = badge;
            _badgeImage.BindingContext = badge;
            _badgeDescription.BindingContext = badge;
        }

        void SetupUIElements()
        {
            _badgeName = new Label
            {
                FontSize = 20
            };
            _badgeName.SetBinding(Label.TextProperty, "Name");

            _badgeImage = new Image
            { 
                HeightRequest = 150,
                WidthRequest = 150
            };
            _badgeImage.SetBinding(
                Image.SourceProperty, "Image",
                converter: new ByteArrayToImageSourceConverter());

            _badgeDescription = new Label
            {
                FontSize = 20,
            };
            _badgeDescription.SetBinding(Label.TextProperty, "Description");
        }

        private async void OnDismissPopup(object? sender, EventArgs e)
        {
            _popupService.HidePopup();
        }
    }
}