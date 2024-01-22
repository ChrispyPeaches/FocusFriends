using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Resources.FontAwesomeIcons;
using Sharpnado.Tabs;

namespace FocusApp.Views
{
    internal class TimerView : ContentView
    {
        public TimerView()
        {
            Content = new Grid
            {
                BackgroundColor = Colors.PeachPuff,
                Children =
                {
                    new Button
                    {
                        Text = SolidIcons.Gears,
                        TextColor = Colors.Black,
                        FontFamily = nameof(SolidIcons),
                        FontSize = 40,
                        BackgroundColor = Colors.Transparent
                    }
                    .Top()
                    .Left()
                    .Invoke(b => b.Clicked += (sender, e) => { Content = new SettingsView(); }),

                    new Label
                    {
                        Text = "Timer",
                        FontSize = 30,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                }
            };
        }
    }
}
