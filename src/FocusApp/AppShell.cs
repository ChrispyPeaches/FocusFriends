using CommunityToolkit.Maui.Markup;
using FocusApp.Views;
using FocusApp.Helpers;
using FocusApp.Resources.FontAwesomeIcons;
using FocusApp.Resources;
using Microsoft.Maui.Controls;
using SimpleToolkit.SimpleShell;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace FocusApp;

public class AppShell : SimpleShell
{
    List<Button> tabButtons;

    public AppShell()
    {
        // Create tabs, all pages must exist in a tab and be added to the tab bar to be navigated to
        var shopTab = new Tab()
        {
            Title = "ShopView",
            Route = "ShopView",
            Items = {
             new ShellContent()
             {
                 Title = "ShopView",
                 ContentTemplate = new DataTemplate(() => new Views.Shop.MainView()),
                 Route = "ShopView"
             }
         }
        };
        var timerTab = new Tab()
        {
            Title = "TimerView",
            Route = "TimerView",
            Items =
            {
                new ShellContent()
                {
                     Title = "TimerView",
                     ContentTemplate = new DataTemplate(() => new TimerView()),
                     Route = "TimerView"
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
                     ContentTemplate = new DataTemplate(() => new Views.Social.MainPage()),
                     Route = "SocialPage"
                }
            }
        };
        var settingsTab = new Tab()
        {
            Title = "SettingsView",
            Route = "SettingsView",
            Items =
            {
                new ShellContent()
                {
                     Title = "SettingsView",
                     ContentTemplate = new DataTemplate(() => new SettingsView()),
                     Route = "SettingsView"
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
            Opacity = 0.8,
            Text = SolidIcons.BagShopping
        }
        .Column(0)
        .FillHorizontal()
        .FillVertical()
        .Invoke(button => button.Released += (sender, eventArgs) =>
            ShellItemButtonClicked(sender, eventArgs));

        var timerButton = new Button
        {
            BindingContext = timerTab,
            CornerRadius = 20,
            BackgroundColor = Colors.White,
            Padding = 0,
            FontSize = 30,
            FontFamily = nameof(SolidIcons),
            TextColor = Colors.Black,
            Opacity = 0.8,
            Text = SolidIcons.Clock
        }
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
            Opacity = 0.8,
            Text = SolidIcons.Users
        }
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
            BackgroundColor = Color.FromArgb("#C8B6FF"),

            // Define the rows
            RowDefinitions = Rows.Define(Stars(9), Star),

            Children =
            {
                // The simple navigation host is the "window" that lets you see the pages
                new SimpleNavigationHost(),

                // This grid contains the tab buttons
                new Grid
                {
                    BackgroundColor = Color.FromArgb("#C8B6FF"),

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
        tabbar.Items.Add(settingsTab);
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
        button.BackgroundColor = Colors.White;

        // Navigate to a new tab if it is not the current tab
        if (!CurrentState.Location.OriginalString.Contains(shellItem.Route))
            await GoToAsync($"///{shellItem.Route}");
    }
}