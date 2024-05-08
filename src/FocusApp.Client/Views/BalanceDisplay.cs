using Microsoft.Maui.Layouts;
using CommunityToolkit.Maui.Markup;

namespace FocusApp.Client.Views
{
    internal class BalanceDisplay : FlexLayout
    {

        public string BalanceAmount
        {
            get => (string)GetValue(BalanceAmountProperty);
            set => SetValue(BalanceAmountProperty, value);
        }

        /// <summary>Bindable property for <see cref="BalanceAmount"/>.</summary>
        public static readonly BindableProperty BalanceAmountProperty = BindableProperty.Create(
            propertyName: nameof(BalanceAmount),
            returnType: typeof(string),
            declaringType: typeof(BalanceDisplay));

        public BalanceDisplay()
        {
            JustifyContent = FlexJustify.End;
            AlignItems = FlexAlignItems.Center;
            HeightRequest = 30;

            // Currency text
            Children.Add(
                new Label
                    {
                        Text = BalanceAmount,
                        FontSize = 23,
                    }
                    .Bind(Label.TextProperty,
                        getter: static (balanceDisplay) => balanceDisplay.BalanceAmount,
                        source: this));

            // Currency icon
            Children.Add(
                new Image
                    {
                        Source = new FileImageSource
                        {
                            File = "coin.png"
                        },
                        HeightRequest = 30,
                        WidthRequest = 30,
                    }
                    .Margins(left: 10));
        }
    }
}
