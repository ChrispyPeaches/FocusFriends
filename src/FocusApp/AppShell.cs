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
        Routing.RegisterRoute(nameof(Views.Social.MainPage), typeof(Views.Social.MainPage));

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


        RootPageContainer = new Grid()
        {
            // Define tab container background color
            BackgroundColor = Color.FromArgb("#C8B6FF"),

            // Define the rows & columns
            RowDefinitions = Rows.Define(Star, Auto),

            Children =
            {
                new SimpleNavigationHost(),
                new HorizontalStackLayout
                {
                    BackgroundColor = Color.FromArgb("#C8B6FF"),
                    HorizontalOptions = LayoutOptions.Center,
                    Spacing = 30,

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
                        }
                        .Invoke(button => button.Released += (sender, eventArgs) =>
                            ShellItemButtonClicked(sender, eventArgs)),

                        new Button
                        {
                            Text = "Social",
                            BindingContext = socialTab
                        }
                        .Invoke(button => button.Released += (sender, eventArgs) =>
                            ShellItemButtonClicked(sender, eventArgs)),

                        new Frame
                        {
                            CornerRadius = 20,
                            HasShadow = false,
                            BackgroundColor = Colors.Transparent,
                            Padding = 0,
                            CascadeInputTransparent = true,
                            InputTransparent = true,
                            Content = new Label()
                            {
                                FontSize = 30,
                                FontFamily = nameof(SolidIcons),
                                TextColor = Colors.Black,
                                Opacity = 0.8,
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                InputTransparent = true,
                                Text = SolidIcons.Clock
                            },
                            GestureRecognizers = { new TapGestureRecognizer() }
                        }
                        .FillVertical()
                        .FillHorizontal()
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
        tabbar.Items.Add(settingsTab);
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