using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Views;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Resources;
using Microsoft.Maui.Controls;
using SimpleToolkit.SimpleShell;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Clients;
using FocusApp.Client.Views.Shop;
using FocusApp.Client.Views.Social;
using FocusApp.Shared.Data;
using Microsoft.EntityFrameworkCore;
using SimpleToolkit.SimpleShell.Extensions;

namespace FocusApp.Client;

public class AppShell : SimpleShell
{
    List<Button> tabButtons;
    public AppShell()
    {
        // Register routes to any side pages
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(PetsPage), typeof(PetsPage));

        this.SetTransition(Transitions.RightToLeftPlatformTransition);

        // Create tabs, tab pages must exist in a tab and be added to the tab bar to be navigated to
        var shopTab = new Tab()
        {
            Title = "ShopPage",
            Route = "ShopPage",
            Items = {
             new ShellContent()
             {
                 Title = "ShopPage",
                 ContentTemplate = new DataTemplate(typeof(MainPage)),
                 Route = "ShopPage"
             }
         }
        };
        var timerTab = new Tab()
        {
            Title = "TimerPage",
            Route = "TimerPage",
            Items =
            {
                new ShellContent()
                {
                     Title = "TimerPage",
                     ContentTemplate = new DataTemplate(typeof(TimerPage)),
                     Route = "TimerPage"
                }
            }
        };
        var socialTab = new Tab()
        {
            Title = "SocialPage",
            Route = "SocialPage",
            Items =
            {
                new ShellContent()
                {
                     Title = "SocialPage",
                     ContentTemplate = new DataTemplate(typeof(SocialPage)),
                     Route = "SocialPage"
                }
            }
        };

        // Create buttons and add to button list
        #region Buttons
        var shopButton = new Button
        {
            BindingContext = shopTab,
            CornerRadius = 20,
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            FontSize = 30,
            FontFamily = nameof(SolidIcons),
            TextColor = Colors.Black,
            Text = SolidIcons.BagShopping
        }
        .Paddings(top: 10, bottom: 10, left: 15, right: 15)
        .Column(0)
        .FillHorizontal()
        .FillVertical()
        .Invoke(button => button.Released += (sender, eventArgs) =>
            ShellItemButtonClicked(sender, eventArgs));

        var timerButton = new Button
        {
            BindingContext = timerTab,
            CornerRadius = 20,
            BackgroundColor = AppStyles.Palette.LightMauve,
            Padding = 0,
            FontSize = 30,
            FontFamily = nameof(SolidIcons),
            TextColor = Colors.Black,
            Text = SolidIcons.Clock
        }
        .Paddings(top: 10, bottom: 10, left: 15, right: 15)
        .Column(1)
        .FillHorizontal()
        .FillVertical()
        .Invoke(button => button.Released += (sender, eventArgs) =>
            ShellItemButtonClicked(sender, eventArgs));

        var socialButton = new Button
        {
            BindingContext = socialTab,
            CornerRadius = 20,
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            FontSize = 30,
            FontFamily = nameof(SolidIcons),
            TextColor = Colors.Black,
            Text = SolidIcons.Users
        }
        .Paddings(top: 10, bottom: 10, left: 15, right: 15)
        .Column(2)
        .FillHorizontal()
        .FillVertical()
        .Invoke(button => button.Released += (sender, eventArgs) =>
            ShellItemButtonClicked(sender, eventArgs));
        #endregion

        tabButtons = new List<Button> { shopButton, timerButton, socialButton };

        // This container holds all the view data
        RootPageContainer = new Grid()
        {
            // Define tab container background color
            BackgroundColor = Colors.Transparent,

            // Define the rows
            RowDefinitions = Rows.Define(Stars(9), Star),

            Children =
            {
                // The simple navigation host is the "window" that lets you see the pages
                new SimpleNavigationHost()
                {
                    ZIndex = 0
                }
                .RowSpan(2),

                // This grid contains the tab buttons
                new Grid
                {
                    ZIndex = 1,
                    BackgroundColor = Colors.Transparent,

                    // Define the rows
                    ColumnDefinitions = Columns.Define(Star, Stars(2), Star),

                    Children =
                    {
                        tabButtons[0],
                        tabButtons[1],
                        tabButtons[2]
                    }
                }
                .Row(1)
                .Margin(20, 5)
            }
        };

        // Tabs must be added to the tab bar in order to be routed
        var tabbar = new TabBar() { Title = "Tabbar", Route = "Tab" };
        tabbar.Items.Add(timerTab);
        tabbar.Items.Add(shopTab);
        tabbar.Items.Add(socialTab);
        Items.Add(tabbar);
    }

    /// <summary>
    /// This on click function navigates to the page attached to the route contained in the shell item
    /// </summary>
    private async void ShellItemButtonClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var shellItem = button.BindingContext as BaseShellItem;

        // Reset highlighted tab button color
        for (int i = 0; i < tabButtons.Count(); i++)
        {
            tabButtons[i].BackgroundColor = Colors.Transparent;
        }

        // Highlight current selected tab
        button.BackgroundColor = AppStyles.Palette.LightMauve;

        // Navigate to a new tab if it is not the current tab
        if (!CurrentState.Location.OriginalString.Contains(shellItem.Route))
        {
            // Determine navigation animation
            switch (shellItem.Route)
            {
                case "ShopPage":
                    this.SetTransition(Transitions.LeftToRightPlatformTransition);
                    break;
                case "TimerPage":
                    if (Shell.Current.CurrentItem.CurrentItem.Route == "ShopPage")
                    {
                        this.SetTransition(Transitions.RightToLeftPlatformTransition);
                    }
                    else if (Shell.Current.CurrentItem.CurrentItem.Route == "SocialPage")
                    {
                        this.SetTransition(Transitions.LeftToRightPlatformTransition);
                    }
                    break;
                case "SocialPage":
                    this.SetTransition(Transitions.RightToLeftPlatformTransition);
                    break;
                // default animation is simple fade in
                default:
                    this.SetTransition(null);
                    break;
            }

            await GoToAsync($"///{shellItem.Route}", true);
        }
    }
}