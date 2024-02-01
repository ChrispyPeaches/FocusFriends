using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Resources.FontAwesomeIcons;

namespace FocusApp.Views.Social
{
    public class AccountPage : ContentPage
    {
        public AccountPage() 
        {
            Content = new Grid
            {
                BackgroundColor = Colors.PeachPuff,
                Children =
                {
                    new Label
                    {
                        Text = "Account",
                        FontSize = 30,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    },
                    // Back Button
                    new Button
                    {
                         Text = SolidIcons.ChevronLeft,
                         TextColor = Colors.Black,
                         FontFamily = nameof(SolidIcons),
                         FontSize = 40,
                         BackgroundColor = Colors.Transparent
                    }
                    .Left()
                    .CenterVertical()
                    .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                    .Column(0)
                    // When clicked, go to timer view
                    //.Invoke(b => b.Clicked += (sender, e) => {Console.WriteLine("Back Button Tapped");}),
                    .Invoke(b => b.Clicked += (sender, e) => { Content = new MainView(); }),

                    new Button
                    {
                        Text = SolidIcons.BreadSlice,
                        TextColor = Colors.Black,
                        FontFamily = nameof(SolidIcons),
                        FontSize = 40,
                        BackgroundColor = Colors.Transparent
                    }
                    .Row(1)
                    .Column(0)
                    .Invoke(b => b.Clicked += (sender, e) => { Content = new EditAccountView(); }),
                }
            };
        }
    }
}