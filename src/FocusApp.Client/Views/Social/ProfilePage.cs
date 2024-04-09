using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using Microsoft.Maui.Layouts;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Views.Controls;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using SimpleToolkit.SimpleShell.Extensions;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using System.Security.Cryptography.X509Certificates;
using MediatR;
using FocusCore.Extensions;


namespace FocusApp.Client.Views.Social;

internal class ProfilePage : BasePage
{
    IAPIClient _client;
    IAuthenticationService _authenticationService;
    PopupService _popupService;
    FocusAppContext _localContext;

    #region Frontend
    public ProfilePage(IAPIClient client, IAuthenticationService authenticationService, PopupService popupService, FocusAppContext localContext) 
    {
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _localContext = localContext;

        #region Pet FlexLayout Population
        var petsFlexLayout = new FlexLayout
        {
            Direction = FlexDirection.Row,
            Wrap = FlexWrap.Wrap,
            JustifyContent = FlexJustify.SpaceEvenly,
            AlignItems = FlexAlignItems.Center,
            AlignContent = FlexAlignContent.Center
        };

        foreach (var pet in _authenticationService.CurrentUser?.Pets)
        {
            var petImage = new Image
            {
                Source = new ByteArrayToImageSourceConverter().ConvertFrom(pet.Pet?.Image),
                HeightRequest = 40,
                WidthRequest = 100,
                Aspect = Aspect.AspectFill
            };

            petsFlexLayout.Children.Add(petImage);
        }
        #endregion

        #region Badge FlexLayout Population
        var badgesFlexLayout = new FlexLayout
        {
            Direction = FlexDirection.Row,
            Wrap = FlexWrap.Wrap,
            JustifyContent = FlexJustify.SpaceEvenly,
            AlignItems = FlexAlignItems.Center,
            AlignContent = FlexAlignContent.Center
        };

        foreach (var badge in _authenticationService.CurrentUser?.Badges)
        {
            var badgeImage = new Image
            {
                Source = new ByteArrayToImageSourceConverter().ConvertFrom(badge.Badge.Image),
                HeightRequest = 40,
                WidthRequest = 100,
                Aspect = Aspect.AspectFill
            };

            petsFlexLayout.Children.Add(badgeImage);
        }
        #endregion

        // Using grids
        Content = new Grid
        {
            // Define the length of the rows & columns
            ColumnDefinitions = Columns.Define(Star, Star, Star),
            RowDefinitions = Rows.Define(Star, Star, Star, Star, Star, Star),
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
                .Top()
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .Column(0)
                .Row(0)
                // When clicked, go to timer view
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    BackButtonClicked(sender, eventArgs)),

                // Profile Picture
                new Frame
                {
                    HeightRequest = 70,
                    WidthRequest = 70,
                    BackgroundColor = Colors.Transparent,
                    VerticalOptions = LayoutOptions.Center,
                    CornerRadius = 28,
                    Content = new Image
                    {
                        // TODO Replace logic with user profile called from DB
                        Source = new ByteArrayToImageSourceConverter().ConvertFrom(_authenticationService.CurrentUser?.ProfilePicture)
                    }
                }
                .Column(1)
                .Row(0)
                .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                .Center(),

                // Profile Info
                new StackLayout
                {
                    Spacing = 10,
                    Children =
                    {
                        // @ ID
                        new Label { Text = _authenticationService.CurrentUser?.Email},
                        new Label { Text = _authenticationService.CurrentUser?.FirstName},
                        new Label { Text = _authenticationService.CurrentUser?.Pronouns}
                    }
                }
                .Left()
                .Column(2)
                .Row(0),

                // User Pets
                petsFlexLayout
                .Row(2)
                .Column(0)
                .ColumnSpan(6)
                .Left(),

                // User Badges
                badgesFlexLayout
                .Row(3)
                .Column(0)
                .ColumnSpan(6)
                .Left(),

                // Date Joined
                new Label
                {
                    Text = $"Member Since: {_authenticationService.CurrentUser?.DateCreated.ToShortDateString()}",
                    TextColor = Colors.Black,
                    FontSize = 20
                }
                .Row(4)
                .Column(0)
                .ColumnSpan(6)
            }
        };
    }

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
