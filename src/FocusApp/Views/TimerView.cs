using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Resources.FontAwesomeIcons;

namespace FocusApp.Views
{
    internal class TimerView : ContentPage
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
                    .Invoke(button => button.Released += (sender, eventArgs) =>
                            SettingsButtonClicked(sender, eventArgs)),
                    

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

        private async void SettingsButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///" + nameof(SettingsView));
        }
    }
}
