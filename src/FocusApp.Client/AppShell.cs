using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Views;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Resources;
using SimpleToolkit.SimpleShell;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Views.Shop;
using FocusApp.Client.Views.Social;
using SimpleToolkit.SimpleShell.Extensions;
using Microsoft.Maui;

namespace FocusApp.Client;

public class AppShell : SimpleShell
{
    public List<Button> tabButtons { get; set; }

    private Grid _tabBarGrid;

    public new static AppShell Current => Shell.Current as AppShell;

    enum Row { PageContent, TabBar }
    enum Column { ShopTab, TimerTab, SocialTab }

    public AppShell()
    {

        // Register routes to any side pages
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

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
                 ContentTemplate = new DataTemplate(typeof(ShopPage)),
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
                },

                new ShellContent()
                {
                     Title = "PetsPage",
                     ContentTemplate = new DataTemplate(typeof(PetsPage)),
                     Route = "PetsPage"
                },

                new ShellContent
                {
                    Title = "FriendProfilePage",
                    ContentTemplate = new DataTemplate(typeof(FriendProfilePage)),
                    Route = "FriendProfilePage"
                },
                new ShellContent()
                {
                    Title = "ProfilePage",
                    ContentTemplate = new DataTemplate (typeof(ProfilePage)),
                    Route = "ProfilePage"
                },

                new ShellContent() 
                {
                    Title = "ProfilePageEdit",
                    ContentTemplate = new DataTemplate(typeof(ProfilePageEdit)),
                    Route = "ProfilePageEdit"
                },
                
                new ShellContent()
                {
                    Title = "LeaderboardsPage",
                    ContentTemplate = new DataTemplate(typeof(LeaderboardsPage)),
                    Route = "LeaderboardsPage"
                },

                new ShellContent()
                {
                    Title = "BadgesPage",
                    ContentTemplate = new DataTemplate (typeof(BadgesPage)),
                    Route = "BadgesPage"
                }
            }
        };
        var loginTab = new Tab()
        {
            Title = "LoginPage",
            Route = "LoginPage",
            Items =
            {
                new ShellContent()
                {
                    Title = "LoginPage",
                    ContentTemplate = new DataTemplate(typeof(LoginPage)),
                    Route = "LoginPage"
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

        _tabBarGrid = new Grid
            {
                ZIndex = 1,
                BackgroundColor = Colors.Transparent,

                // Define the rows
                ColumnDefinitions = Columns.Define(
                    (Column.ShopTab, Star),
                    (Column.TimerTab, Stars(2)),
                    (Column.SocialTab, Star)),

                Children =
                {
                    tabButtons[0],
                    tabButtons[1],
                    tabButtons[2]
                }
            };

        // This container holds all the view data
        RootPageContainer = new Grid()
        {
            // Define tab container background color
            BackgroundColor = Colors.Transparent,

            // Define the rows
            RowDefinitions = Rows.Define(
                (Row.PageContent, GridRowsColumns.Stars(9)),
                (Row.TabBar, Consts.TabBarHeight)),

            Children =
            {
                // The simple navigation host is the "window" that lets you see the pages
                new SimpleNavigationHost()
                {
                    ZIndex = 0
                }
                .RowSpan(2),

                // This grid contains the tab buttons
                _tabBarGrid
                .Row(Row.TabBar)
                .Margin(20, 5)
            }
        };

        // Tabs must be added to the tab bar in order to be routed
        var tabbar = new TabBar() { Title = "Tabbar", Route = "Tab" };
        tabbar.Items.Add(loginTab);
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

    /// <summary>
    /// Show or hide the tab bar
    /// </summary>
    public async Task SetTabBarIsVisible(bool newIsVisible)
    {
        
        if (newIsVisible && !_tabBarGrid.IsVisible)
        {
            _tabBarGrid.IsVisible = true;
            await _tabBarGrid.TranslateTo(x: 0, y: 0, easing: Easing.CubicInOut);
        }
        else if (!newIsVisible && _tabBarGrid.IsVisible)
        {
            await _tabBarGrid.TranslateTo(x: 0, y: Consts.TabBarHeight, easing: Easing.CubicInOut);
            _tabBarGrid.IsVisible = false;
        }
    }
}