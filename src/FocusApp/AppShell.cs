using CommunityToolkit.Maui.Markup;
using FocusApp.Views;
using FocusApp.Helpers;
using FocusApp.Resources.FontAwesomeIcons;
using FocusApp.Resources;
using Microsoft.Maui.Controls;
using SimpleToolkit.SimpleShell;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace FocusApp;

internal class AppShell : SimpleShell
{
    private int _selectedTabIndex { get; set; } = 1;

    public AppShell()
    {
        var tab = new Tab()
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
        var tab2 = new Tab()
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
        var grid = new Grid()
        {
            // Define the lenth of the rows & columns
            RowDefinitions = Rows.Define(50, Star),
            ColumnDefinitions = Columns.Define(50, Star),

            Children =
            {
                // Header
                new HorizontalStackLayout
                {
                    
                }
                .Row(0)
                .Column(1)
            }
        };

        var rootPageContainer = RootPageContainer
        {
            Items.Add()
        };

        var tabbar = new TabBar() { Title = "Tabbar", Route = "Tab" };
        tabbar.Items.Add(tab2);
        tabbar.Items.Add(tab);
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