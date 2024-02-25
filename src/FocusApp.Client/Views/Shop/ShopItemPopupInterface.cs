using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using CommunityToolkit.Maui.Views;
using FocusApp.Client.Resources;
using FocusApp.Shared.Models;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics.Text;
using SimpleToolkit.SimpleShell.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusApp.Client.Views.Shop
{
    internal class ShopItemPopupInterface : BasePopup
    {
        private Helpers.PopupService _popupService;
        StackLayout _popupContentStack;


        public ShopItemPopupInterface(Helpers.PopupService popupService)
        {
            _popupService = popupService;

            // Set popup location
            HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
            Color = Colors.Transparent;

            _popupContentStack = new StackLayout();

            Content = new Border
            {
                StrokeThickness = 1,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20,20,20,20)},
                BackgroundColor = AppStyles.Palette.LightMauve,
                WidthRequest = 360,
                HeightRequest = 460,
                Content = _popupContentStack
            };
        }

        public void PopulatePopup(ShopItem item)
        {
            // Shop item name label
            Label itemName = new Label
            {
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
            .Margins(top: 10);
            itemName.SetBinding(Label.TextProperty, "Name");
            itemName.BindingContext = item;

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
            Image itemImage = new Image
            {
                WidthRequest = 150,
                HeightRequest = 150
            };
            itemImage.SetBinding(
                Image.SourceProperty, "ImageSource",
                converter: new ByteArrayToImageSourceConverter());
            itemImage.BindingContext = item;

            // Shop item price
            Label itemPrice = new Label
            {
                FontSize = 30,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
            .Margins(top: 10);
            itemPrice.SetBinding(Label.TextProperty, "Price");
            itemPrice.BindingContext = item;
            
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
                    .Invoke(button => button.Pressed += (s,e) => PageButtonClicked(s,e)),
                    new Button
                    {
                        WidthRequest = 125,
                        HeightRequest = 50,
                        Text = "Buy Me!",
                        HorizontalOptions = LayoutOptions.End,
                    }
                    .Margins(right: 35, top: 50)
                    .Invoke(button => button.Pressed += (s,e) => PageButtonClicked(s,e))
                }
            };

            _popupContentStack.Add(itemName);
            _popupContentStack.Add(divider);
            _popupContentStack.Add(itemImage);
            _popupContentStack.Add(itemPrice);
            _popupContentStack.Add(popupButtons);
        }

        // Navigate to page according to button
        private async void PageButtonClicked(object sender, EventArgs e)
        {
            _popupService.HidePopup();
        }
    }
}
