using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources.FontAwesomeIcons;
using System.Runtime.CompilerServices;
using SimpleToolkit.SimpleShell.Extensions;
using Microsoft.Maui.Layouts;
using CommunityToolkit.Maui.Converters;
using FocusApp.Client.Views.Shop;
using FocusCore.Models;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Client.Methods.User;
using MediatR;
using FocusCore.Commands.User;
using FocusCore.Responses;
using FocusApp.Methods.User;
using FocusApp.Client.Resources;
using Microsoft.Maui.Controls.Shapes;

namespace FocusApp.Client.Views.Social;

internal sealed class DecorPage : BasePage
{
    private readonly IAPIClient _client;
    private readonly IAuthenticationService _authenticationService;
    private readonly PopupService _popupService;
    private readonly FocusAppContext _localContext;
    private readonly IMediator _mediator;

    FlexLayout _decorContainer;
    Image _selectedCheckmark;
    Label _responseMessage;

    public DecorPage(IAPIClient client, IAuthenticationService authenticationService, PopupService popupService, FocusAppContext localContext, IMediator mediator)
	{
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _localContext = localContext;
        _mediator = mediator;

        // Instantiate container for decor
        _decorContainer = new FlexLayout
        {
            Direction = FlexDirection.Row,
            Wrap = FlexWrap.Wrap,
            JustifyContent = FlexJustify.SpaceAround
        };

        // Create label for displaying system responses
        _responseMessage = new Label
        {
            FontSize = 18,
            TextColor = Colors.Black
        };

        // Using grids
        Content = new Grid
        {
            // Background Color
            BackgroundColor = AppStyles.Palette.LightPeriwinkle,

            // Define rows and columns
            RowDefinitions = Rows.Define(80, 5, Star, 28, 80),
            ColumnDefinitions = Columns.Define(Star, Star),

            Children = {

				// Header
				new Label
                {
                    Text = "Decor",
                    TextColor = Colors.Black,
                    FontSize = 40
                }
                .Row(0)
                .Column(0)
                .ColumnSpan(3)
                .CenterVertical()
                .Paddings(top : 5, bottom: 5, left: 75, right: 5),


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
                .Column(0)
                .Paddings(top : 5, bottom: 5, left: 10, right: 5)
				// When clicked, go to social view
				.Invoke(button => button.Released += (sender, eventArgs) =>
                    BackButtonClicked(sender, eventArgs)),

				// Header & Content Divider
				new BoxView
                {
                    Color = Colors.Black,
                    WidthRequest = 400,
                    HeightRequest = 2
                }
                .Bottom()
                .Row(0)
                .Column(0)
                .ColumnSpan(int.MaxValue),
                //////////////////////////////////////////////////////////

				// FlexLayout - Container for Decor
                new ScrollView
                {
                    Content = _decorContainer
                }
                .Row(2)
                .Column(0)
                .ColumnSpan(2)
                .Top()
                .Paddings(bottom: 10),

                // Label for response message
                _responseMessage
                .Row(3)
                .Column(0)
                .ColumnSpan(2)
                .Center()
            }
		};
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Check if the user is logged in
        if (_authenticationService.CurrentUser == null)
        {
            var loginPopup = (EnsureLoginPopupInterface)_popupService.ShowAndGetPopup<EnsureLoginPopupInterface>();
            loginPopup.OriginPage = nameof(ShopPage);
            return;
        }

        // Clear message label
        _responseMessage.Text = "";

        // Fetch decor from the local database and display them
        var userDecor = await FetchDecorFromLocalDb();
        DisplayDecor(userDecor);
    }

    private async Task<List<DecorItem>> FetchDecorFromLocalDb()
    {
        List<DecorItem> userDecor = new List<DecorItem>();

        try
        {
            // Fetch decor from the local database using Mediatr feature
            GetUserDecor.Result result = await _mediator.Send(new GetUserDecor.Query()
            {
                UserId = _authenticationService.CurrentUser.Id,
                selectedDecorId = _authenticationService.SelectedDecor != null ? _authenticationService.SelectedDecor.Id : null
            },
            default);

            userDecor = result.Decor;
        }
        catch (Exception ex)
        {
            throw new Exception("Error when fetching decor from local DB.", ex);
        }

        return userDecor;
    }

    private void DisplayDecor(List<DecorItem> userDecorItems)
    {
        var decorContainer = _decorContainer;

        // Clear container
        _decorContainer.Children.Clear();

        // Add decor to FlexLayout
        foreach (var decor in userDecorItems)
        {
            // Decor Background
            var background = new Border
            {
                Stroke = Colors.Black,
                StrokeThickness = 4,
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(20, 20, 20, 20) },
                BackgroundColor = Colors.Transparent,
                WidthRequest = 170,
                HeightRequest = 170
            };

            // Checkmark for selected decor
            var checkmark = new Image
            {
                Source = "pet_selected.png",
                WidthRequest = 60,
                HeightRequest = 60,
                Opacity = decor.isSelected ? 1.0 : 0.0
            };

            // Assign currently selected checkmark
            if (decor.isSelected)
            {
                _selectedCheckmark = checkmark;
            }

            // Decor Image
            var userDecor = new ImageButton
            {
                Source = ImageSource.FromStream(() => new MemoryStream(decor.DecorPicture)),
                Aspect = Aspect.AspectFit,
                WidthRequest = 130,
                HeightRequest = 130,
                BindingContext = checkmark
            }
            .Invoke(button => button.Released += (s, e) => OnImageButtonClicked(s, e));

            // Decor Name
            var decorName = new Label
            {
                Text = decor.DecorName,
                TextColor = Colors.Black,
                FontSize = 14
            };

            // Create frame with decor inside
            var grid = new Grid
            {
                RowDefinitions = Rows.Define(160,25),
                ColumnDefinitions = Columns.Define(160),
                BindingContext = decor.DecorId,
                Children = 
                {
                    background
                    .Column(0)
                    .Row(0)
                    .ZIndex(0),

                    userDecor
                    .Column(0)
                    .Row(0)
                    .ZIndex(2),

                    checkmark
                    .Column(0)
                    .Row(0)
                    .ZIndex(3)
                    .Top()
                    .Right(),

                    decorName
                    .Column(0)
                    .Row(1)
                    .Center()
        }
            }
            .Paddings(top: 5, bottom: 5);

            decorContainer.Children.Add(grid);
        }
    }

    private async void OnImageButtonClicked(object sender, EventArgs eventArgs)
    {
        var itemButton = sender as ImageButton;

        // Get DecorId from grid binding context
        var grid = itemButton.Parent as Grid;
        var decorId = (Guid)grid.BindingContext;

        // Check if decor is already selected
        if ((_authenticationService.SelectedDecor != null) && (_authenticationService.SelectedDecor.Id == decorId))
        {
            _responseMessage.Text = "Already selected";
            return;
        }

        // Get checkmark from image button bind
        var checkmark = (Image)itemButton.BindingContext;

        EditUserSelectedDecorCommand command = new EditUserSelectedDecorCommand
        {
            UserId = _authenticationService.CurrentUser?.Id,
            DecorId = decorId
        };

        // Call method to update the user data when the user fields have changed
        MediatrResult result = await _mediator.Send(command, default);

        // On success, hide previous checkmark and show new one
        if (result.Success)
        {
            _selectedCheckmark.Opacity = 0.0;
            _selectedCheckmark = checkmark;
            _selectedCheckmark.Opacity = 1.0;

            // Display change message
            _responseMessage.Text = "Selected decor changed to " + _authenticationService.SelectedDecor.Name;
        }
        else
        {
            // Display error message
            _responseMessage.Text = "Server error";
        }
    }

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        // Back navigation reverses animation so can keep right to left
        Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
    }
}
