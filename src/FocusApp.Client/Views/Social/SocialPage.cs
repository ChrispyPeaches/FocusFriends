using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Clients;
using FocusCore.Queries.User;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using SimpleToolkit.SimpleShell.Extensions;
using Microsoft.Maui.Platform;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;

namespace FocusApp.Client.Views.Social;

internal class SocialPage : BasePage
{
    private Helpers.PopupService _popupService;

    IAPIClient _client { get; set; }
	public SocialPage(IAPIClient client, Helpers.PopupService popupService)
	{
        _popupService = popupService;
        _client = client;
        // Add logic to fetch focused friends
        List<ImageCell> focusingFriends = new List<ImageCell>();
        for (int i = 0; i < 5; i++)
        {
            focusingFriends.Add(new ImageCell
            {
                Text = "Friend " + i,
                ImageSource = new FileImageSource
                {
                    // Add logic that gets profile picture from friend user data
                    File = "dotnet_bot.png"
                },
                BindingContext = this
            });
        };

        DataTemplate dataTemplate = new DataTemplate(typeof(ImageCell));
        dataTemplate.SetBinding(ImageCell.TextProperty, "Text");
        dataTemplate.SetBinding(ImageCell.ImageSourceProperty, "ImageSource");

        Content = new Grid
        {
            // Define rows and columns (Star means that row/column will take up the remaining space)
            RowDefinitions = Rows.Define(80, Star, Star, Star),
            ColumnDefinitions = Columns.Define(Star, Star),
            BackgroundColor = Colors.LightGreen,
            Opacity = 0.9,

            Children =
            {
                // Header
                new Label
                {
                    TextColor = Colors.Black,
                    Text = "Friends",
                    FontSize = 40
                }
                .Row(0)
                .Column(0)
                .Padding(15, 15),

                new Button
                {
                    BackgroundColor = Colors.Transparent,
                    FontFamily = nameof(SolidIcons),
                    TextColor = Colors.Black,
                    Text = SolidIcons.PersonCirclePlus,
                    FontSize = 35
                }
                .Row(0)
                .Column(1)
                .Left()
                .Padding(15, 15)
                .Invoke(b => b.Clicked += (s,e) => OnClickShowAddFriendsPopup(s,e)),

                // Horizontal Divider
                new BoxView
                {
                    Color = Colors.Black,
                    WidthRequest = 400,
                    HeightRequest = 2
                }
                .Bottom()
                .Row(0)
                .Column(0)
                .ColumnSpan(2),

                // Profile Picture
                new ImageButton
                {
                    Source = new FileImageSource
                    {
                        // Add logic that gets profile picture from user data
                        File = "dotnet_bot.png"
                    },
                    WidthRequest = 90,
                    HeightRequest = 90
                }
                .Top()
                .Right()
                .Column(1)
                .Clip(new EllipseGeometry { Center = new Point(43, 45), RadiusX = 27, RadiusY = 27 })
                .Invoke(b => b.Clicked += (s,e) => OnClickShowProfileInterfacePopup(s,e)),

                // Friends List
                new ListView
                {
                    Header = "Focusing",
                    ItemsSource = focusingFriends,
                    ItemTemplate = dataTemplate
                }
                .Row(1)
                .Column(0)
                .ColumnSpan(2)
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }

    // Display navigation popup on hit
    private void OnClickShowProfileInterfacePopup(object sender, EventArgs e)
    {
        _popupService.ShowPopup<ProfilePopupInterface>();
    }

    // Display new friend popup on hit
    private void OnClickShowAddFriendsPopup(object sender, EventArgs e)
    {
        _popupService.ShowPopup<AddFriendPopupInterface>();
    }
}