using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Shared.Data;
using SimpleToolkit.SimpleShell.Extensions;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace FocusApp.Client.Views.Social;

internal class ProfilePage : BasePage
{
    IAPIClient _client;
    IAuthenticationService _authenticationService;
    PopupService _popupService;
    FocusAppContext _localContext;
    CarouselView _petsCarouselView { get; set; }
    CarouselView _badgesCarouselView { get; set; }

    #region Frontend
    public ProfilePage(IAPIClient client, IAuthenticationService authenticationService, PopupService popupService, FocusAppContext localContext) 
    {
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _localContext = localContext;

        _petsCarouselView = BuildBaseCarouselView();
        _badgesCarouselView = BuildBaseCarouselView();

        // Using grids
        Content = new Grid
        {
            // Define the length of the rows & columns
            RowDefinitions = Rows.Define(),
            ColumnDefinitions = Columns.Define(),
            BackgroundColor = AppStyles.Palette.LightMauve,

            Children =
            {
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
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    BackButtonClicked(sender, eventArgs)),
            }
        };
    }

    private CarouselView BuildBaseCarouselView()
    {
        CarouselView carouselView = new CarouselView();

        carouselView.ItemTemplate = new DataTemplate(() =>
        {
            // Shop item name label
            Label itemName = new Label
            {
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            itemName.SetBinding(Label.TextProperty, "Name");

            // Shop item image
            ImageButton itemImage = new ImageButton
            {
                WidthRequest = 100,
                HeightRequest = 100,
            };
            itemImage.SetBinding(
                ImageButton.SourceProperty, "ImageSource",
                converter: new ByteArrayToImageSourceConverter());

            itemImage.Clicked += (s, e) =>
            {
                //OnImageButtonClicked(s, e);
            };

            // Shop item price
            Label itemPrice = new Label
            {
                FontSize = 15,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            itemPrice.SetBinding(Label.TextProperty, "Price");

            // Shop item stack for grouping shop item elements
            VerticalStackLayout itemStack = new VerticalStackLayout
            {
                BackgroundColor = Color.FromRgba("#E0E4FF00"),
                Padding = 10
            };

            itemStack.Add(itemName);
            itemStack.Add(itemImage);
            itemStack.Add(itemPrice);

            return itemStack;
        });

        // Add space between the carousels - allows room for carousel label
        carouselView.Margins(0, 5, 0, 10);

        carouselView.BackgroundColor = AppStyles.Palette.MintGreen;

        return carouselView;
    }

    // TODO: Register route in AppShell According to whichever method
    private async void BackButtonClicked(object sender, EventArgs e)
    {
        // Back navigation reverses animation so can keep right to left
        Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
    }
    #endregion

    #region Backend
    #endregion
}
