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
    public AppShell()
    {
        Routing.RegisterRoute(nameof(Views.Social.MainView), typeof(Views.Social.MainView));

        var shopTab = new Tab()
        {
            Title = "SettingsView",
            Route = "SettingsView",
            Items = {
             new ShellContent()
             {
                 Title = "SettingsView",
                 ContentTemplate = new DataTemplate(() => new SettingsView()),
                 Route = "SettingsView"
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
            Title = "SocialView",
            Route = "Views/Social/MainView",
            Items =
            {
                new ShellContent()
                {
                     Title = "SocialView",
                     ContentTemplate = new DataTemplate(() => new Views.Social.MainView()),
                     Route = "Views/Social/MainView"
                }
            }
        };


        RootPageContainer = new Grid()
        {
            // Define the rows & columns
            RowDefinitions = Rows.Define(Star, Auto),

            Children =
            {
                new SimpleNavigationHost(),
                new HorizontalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 10,

                    Children =
                    {
                        new Button
                        {
                            Text = "Shop",
                            BindingContext = shopTab
                        }
                        .Invoke(button => button.Released += (sender, eventArgs) =>
                            ShellItemButtonClicked(sender, eventArgs)),

                        new Button
                        {
                            Text = "Timer",
                            BindingContext = timerTab
                        }.Invoke(button => button.Released += (sender, eventArgs) =>
                            ShellItemButtonClicked(sender, eventArgs)),

                        new Button
                        {
                            Text = "Social",
                            BindingContext = socialTab
                        }.Invoke(button => button.Released += (sender, eventArgs) =>
                            ShellItemButtonClicked(sender, eventArgs))
                    }
                }
                .Row(1)
                .Margin(20, 5)
            }
        };

        var tabbar = new TabBar() { Title = "Tabbar", Route = "Tab" };
        tabbar.Items.Add(timerTab);
        tabbar.Items.Add(shopTab);
        tabbar.Items.Add(socialTab);
        Items.Add(tabbar);

        Build();
#if DEBUG
        HotReloadService.UpdateApplicationEvent += ReloadUI;
#endif
    }

    /// <summary>
    /// For the main page, the UI has a separate method so it can be rebuilt when Hot Reload is triggered.
    /// </summary>
    public void Build()
    {
        //
    }

    private async void ShellItemButtonClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var shellItem = button.BindingContext as BaseShellItem;

        // Navigate to a new tab if it is not the current tab
        if (!CurrentState.Location.OriginalString.Contains(shellItem.Route))
            await GoToAsync($"///{shellItem.Route}");
    }

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        await GoToAsync("..");
    }

    /// <summary>
    /// This method is called when Hot Reload is triggered.
    /// </summary>
    /// <param name="obj"></param>
    private void ReloadUI(Type[] obj)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Build();
        });
    }
}