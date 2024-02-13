using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Clients;
using FocusCore.Queries.User;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Resources;

namespace FocusApp.Client.Views.Social;

internal class SocialPage : BasePage
{
    Popup navigationPopup;
    IPopupService popupService;

    IAPIClient _client { get; set; }
	public SocialPage(IAPIClient client)
	{
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

        navigationPopup = new Popup
        {
            // Set popup location
            HorizontalOptions = Microsoft.Maui.Primitives.LayoutAlignment.End,
            VerticalOptions = Microsoft.Maui.Primitives.LayoutAlignment.Start,

            Content = new VerticalStackLayout
            {
                BackgroundColor = Colors.Transparent,
                Children =
                {
                    new Frame
                    {
                        CornerRadius = 20,
                        BackgroundColor = AppStyles.Palette.LightMauve,
                        Content = new VerticalStackLayout
                        {
                            Children =
                            {
                                new Label()
                                {
                                    FontSize = 30,
                                    TextColor = Colors.White,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Center,

                                    // Add logic to fetch username
                                    Text = "Username"
                                },

                                new Button()
                                {
                                    BackgroundColor = Colors.Transparent,
                                    Padding = 0,
                                    FontSize = 30,
                                    TextColor = Colors.White,
                                    Text = "My Pets"
                                }
                                .Invoke(button => button.Released += (sender, eventArgs) =>
                                        PageButtonClicked(sender, eventArgs)),
                            }
                        }
                        .Top()
                        .Right()
                    }
                }
            }
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
                    Text = "Friends",
                    FontSize = 40
                }
                .Row(0)
                .Column(0)
                .Padding(15, 15),

                // Horizontal Divider
                new BoxView
                {
                    Color = Color.Parse("Black"),
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
                .Invoke(b => b.Clicked += (s,e) => OnClickShowPopup(s,e)),

                // Friends List
                new ListView
                {
                    Header = "Focusing",
                    ItemsSource = focusingFriends,
                    ItemTemplate = dataTemplate
                }
                .Row(1)
                .Column(0)
                .ColumnSpan(2),

                new Button
                {
                    Text = "Pets Page",
                    MaximumHeightRequest = 50
                }
                .Row(2)
                .Column(0)
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    PetsButtonClicked(sender, eventArgs)),
            }
        };
    }

    private async void PetsButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///" + nameof(PetsPage));
    }

    protected override async void OnAppearing()
    {
        var user = await _client.GetUser(new GetUserQuery { Id = Guid.NewGuid() });
        base.OnAppearing();
    }

    // Display navigation popup on hit
    private void OnClickShowPopup(object sender, EventArgs e)
    {
        popupService.ShowPopup<ProfilePopupInterface>();
    }

    // Navigate to page according to button
    private async void PageButtonClicked(object sender, EventArgs e)
    {
        var button = sender as Button;

        await Shell.Current.GoToAsync("///" + nameof(PetsPage));
        //popupService.();
    }
}