using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Shapes;

namespace FocusApp.Client.Views.Social;

internal class UserBadgesPagePopupInterface : BasePopup
{
    PopupService _popupService;
    StackLayout _popupContentStack;
    IAuthenticationService _authenticationService;
    IFocusAppContext _localContext;
    IAPIClient _client;
    ILogger<UserBadgesPagePopupInterface> _logger;
    Badge _currentBadge { get; set; }
    public UserBadgesPage UserBadgesPage { get; set; }

    public UserBadgesPagePopupInterface(PopupService popupService, IAuthenticationService authenticationService, IFocusAppContext localContext, IAPIClient client, ILogger<UserBadgesPagePopupInterface> logger)
    {
        _popupService = popupService;
        _authenticationService = authenticationService;
        _localContext = localContext;
        _client = client;
        _logger = logger;

        // Set popup location
        HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
        VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Center;
        Color = Colors.Transparent;

        CanBeDismissedByTappingOutsideOfPopup = false;

        _popupContentStack = new StackLayout();

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

    public void PopulatePopup(Badge badge)
    {
        _currentBadge = badge;

        // Badge name label
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
        };

        // Badge image
        Image badgeImage = new Image
        {
            WidthRequest = 150,
            HeightRequest = 150
        };
        badgeImage.SetBinding(
            Image.SourceProperty, "Image",
            converter: new ByteArrayToImageSourceConverter());
        badgeImage.BindingContext = badge;

        // Badge Description
        Label badgeDescription = new Label
        {
            Text = "This is a brief description on how the badge is unlocked",
            FontSize = 10,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        //badgeDescription.SetBinding(Label.TextProperty, "Description");
        //badgeDescription.BindingContext = badge;

        // Date Acquired
        Label dateAcquired = new Label
        {
            Text = "00/00/00",
            FontSize= 15,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        //badgeDescription.SetBinding(Label.TextProperty, "DateAcquired");
        //badgeDescription.BindingContext = badge;

        // Exit Popup Button
        Button exitButton = new Button
        {
            WidthRequest = 125,
            HeightRequest = 50,
            Text = "Close",
            HorizontalOptions = LayoutOptions.Center
        }
        .Margins(top: 50)
        .Invoke(button => button.Released += (s, e) => ExitItemPopup(s, e));

        _popupContentStack.Add(badgeName);
        _popupContentStack.Add(divider);
        _popupContentStack.Add(badgeImage);
        _popupContentStack.Add(badgeDescription);
        _popupContentStack.Add(dateAcquired);
        _popupContentStack.Add(exitButton);
    }

    // User doesn't want to purchase the item, hide popup
    private async void ExitItemPopup(object sender, EventArgs e)
    {
        _popupService.HidePopup();
    }
}
