using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using FocusCore.Responses;
using MediatR;
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
    DateTimeOffset badgeDateAcquired;
    private readonly IMediator _mediator;
    Badge _currentBadge { get; set; }
    public UserBadgesPage UserBadgesPage { get; set; }

    public UserBadgesPagePopupInterface(PopupService popupService, IAuthenticationService authenticationService, IFocusAppContext localContext, IAPIClient client, ILogger<UserBadgesPagePopupInterface> logger, IMediator mediator)
    {
        _popupService = popupService;
        _authenticationService = authenticationService;
        _localContext = localContext;
        _client = client;
        _logger = logger;
        _mediator = mediator;

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

    public async Task PopulatePopup(Badge badge)
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
            FontSize = 15,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        badgeDescription.SetBinding(Label.TextProperty, "Description");
        badgeDescription.BindingContext = badge;

        // Date Acquired
        Label dateAcquired = new Label
        {
            BindingContext = badge,
            Text = "",
            FontSize= 15,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        // Exit Popup Button
        Button selectButton = new Button
        {
            BindingContext = badge,
            WidthRequest = 125,
            HeightRequest = 50,
            Text = "Select",
            HorizontalOptions = LayoutOptions.End
        }
        .Margins(right: 35, top: 50)
        .Invoke(button => button.Released += (s, e) => SelectBadgeButton(s, e));

        if (await UserOwnsItem())
        {
            badgeDateAcquired = await GetBadgeDate();

            if (_authenticationService.SelectedBadge == badge)
            {
                dateAcquired.Text = $"Date Unlocked: {badgeDateAcquired.ToString("d")}";

                selectButton.Text = "Selected";
                selectButton.Opacity = 0.5;
                selectButton.IsEnabled = false;
            }
            else if (_authenticationService.SelectedBadge != badge)
            {
                dateAcquired.Text = $"Date Unlocked: {badgeDateAcquired.ToString("d")}";

                selectButton.Text = "Select";
                selectButton.Opacity = 1;
                selectButton.IsEnabled = true;
            }
        }
        else
        {
            selectButton.Text = "Locked";
            selectButton.Opacity = 0.5;
            selectButton.IsEnabled = false;
        }


        //Grid for both buttons
        Grid popupButtons = new Grid
        {
            Children =
                {
                    new Button
                    {
                        WidthRequest = 125,
                        HeightRequest = 50,
                        Text = "Close",
                        HorizontalOptions = LayoutOptions.Start,
                    }
                    .Margins(left: 35, top: 50)
                    .Invoke(button => button.Pressed += (s,e) => ExitItemPopup(s,e)),
                    selectButton
                }
        };

        _popupContentStack.Add(badgeName);
        _popupContentStack.Add(divider);
        _popupContentStack.Add(badgeImage);
        _popupContentStack.Add(badgeDescription);
        _popupContentStack.Add(dateAcquired);
        _popupContentStack.Add(popupButtons);
    }

    // User doesn't want to purchase the item, hide popup
    private async void ExitItemPopup(object sender, EventArgs e)
    {
        _popupService.HidePopup();
    }

    private async void SelectBadgeButton(object sender, EventArgs e)
    {
        var newBadge = sender as Button;
        var badge = (Badge)newBadge.BindingContext;

        Image checkmark = UserBadgesPage._selectedCheckmark;
        if (checkmark != null)
            UserBadgesPage._selectedCheckmark.Opacity = 0;

        UserBadgesPage._selectedCheckmark = UserBadgesPage.BadgeDict[badge];
        UserBadgesPage._selectedCheckmark.Opacity = 1;

        EditUserSelectedBadgeCommand command = new EditUserSelectedBadgeCommand
        {
            UserId = _authenticationService.CurrentUser?.Id,
            BadgeId = badge.Id
        };

        MediatrResult result = await _mediator.Send(command, default);

        _authenticationService.SelectedBadge = badge;

        _popupService.HidePopup();
    }

    private async Task<bool> UserOwnsItem()
    {
        return await _localContext.UserBadges.AnyAsync(b =>
        b.BadgeId == _currentBadge.Id
        && b.UserId == _authenticationService.CurrentUser.Id);
    }

    private async Task<DateTimeOffset> GetBadgeDate()
    { 
        return (await _localContext.UserBadges.FirstAsync(ub => 
        ub.UserId == _authenticationService.CurrentUser.Id &&
        ub.BadgeId == _currentBadge.Id)).DateAcquired;
    }
}
