using CommunityToolkit.Maui.Markup;
using FocusApp.Views;
using Sharpnado.Tabs;
using FocusApp.Helpers;
using FocusApp.Resources.FontAwesomeIcons;
using FocusApp.Resources;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Maui.Controls;
using Sharpnado.Tabs.Effects;

namespace FocusApp;

internal class MainPage : ContentPage
{
    private int _selectedTabIndex { get; set; } = 1;

    public MainPage()
    {
        Build();
#if DEBUG
        HotReloadService.UpdateApplicationEvent += ReloadUI;
#endif
    }



    #region Generate Tabs

    private static BottomTabItem GenerateBaseTab()
    {
        Frame frame = new Frame()
        {
            CornerRadius = 20,
            HeightRequest = 40,
            HasShadow = false,
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            CascadeInputTransparent = true,
            InputTransparent = true,
            Content = new Label()
            {
                FontSize = 20,
                FontFamily = nameof(SolidIcons),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                InputTransparent = true
            }
        };

        BottomTabItem baseTab = new BottomTabItem()
        {
            HorizontalOptions = LayoutOptions.Center,
            Background = Colors.Transparent,
            BackgroundColor = Colors.Transparent, 
            VerticalOptions = LayoutOptions.Center,
            Content = frame
        };

        // If the tab is selected, show the background color
        baseTab.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(baseTab.IsSelected))
            {
                frame.BackgroundColor = baseTab.IsSelected ?
                    AppStyles.navigationBarButtonBackgroundColor
                    : Colors.Transparent;
            }
        };

        return baseTab;
    }

    public static BottomTabItem GenerateShopTab()
    {
        BottomTabItem shopTab = GenerateBaseTab()
            .Paddings(
                top: 5,
                bottom: 5,
                left: 10,
                right: 10);

        Frame frame = VisualTreeElementExtensions
            .GetVisualTreeDescendants(shopTab)
            .OfType<Frame>()
            .First();
        frame.WidthRequest = 100;

        Label label = VisualTreeElementExtensions
            .GetVisualTreeDescendants(frame)
            .OfType<Label>()
            .First();
        label.Text = SolidIcons.BagShopping;

        return shopTab;
    }

    public static BottomTabItem GenerateTimerTab()
    {
        BottomTabItem timerTab = GenerateBaseTab()
            .Paddings(
                top: 5,
                bottom: 5,
                left: 10,
                right: 10);

        Frame frame = VisualTreeElementExtensions
            .GetVisualTreeDescendants(timerTab)
            .OfType<Frame>()
            .First();
        frame.WidthRequest = 100;

        Label label = VisualTreeElementExtensions
            .GetVisualTreeDescendants(frame)
            .OfType<Label>()
            .First();
        label.Text = SolidIcons.Clock;

        return timerTab;
    }

    public static BottomTabItem GenerateSocialTab()
    {
        BottomTabItem socialTab = GenerateBaseTab()
            .Paddings(
                top: 5,
                bottom: 5,
                left: 10,
                right: 10);

        Frame frame = VisualTreeElementExtensions
            .GetVisualTreeDescendants(socialTab)
            .OfType<Frame>()
            .First();
        frame.WidthRequest = 100;

        Label label = VisualTreeElementExtensions
            .GetVisualTreeDescendants(frame)
            .OfType<Label>()
            .First();
        label.Text = SolidIcons.Users;

        return socialTab;
    }

    #endregion

    /// <summary>
    /// For the main page, we're putting the UI in a separate method so that we can call it again when Hot Reload is triggered.
    /// </summary>
    public void Build()
    {
        TabHostView tabHostView = new TabHostView
        {
            HeightRequest = 60,
            IsSegmented = true, SegmentedHasSeparator = false, SegmentedOutlineColor = Colors.Transparent,
            Orientation = OrientationType.Horizontal,
            TabType = TabType.Fixed,
            VerticalOptions = LayoutOptions.End,
            HorizontalOptions = LayoutOptions.Fill,
            BackgroundColor = Colors.Transparent,
            Tabs =
            {
                GenerateShopTab(),
                GenerateTimerTab(),
                GenerateSocialTab(),
            }
        }
        .Paddings(0, 0);

        ViewSwitcher switcher = new ViewSwitcher()
        {
            Children =
            {
                new DelayedView<Views.Shop.MainView>
                {
                    UseActivityIndicator = true
                },
                new DelayedView<TimerView>
                {
                    UseActivityIndicator = true
                },
                new DelayedView<Views.Social.MainView>
                {
                    UseActivityIndicator = true
                }
            }
        }
        .Margins(0)
        .RowSpan(3);

        tabHostView.SelectedTabIndexChanged += (s, e) =>
        {
            switcher.SelectedIndex = (int)e.SelectedPosition;
        };

        tabHostView.SelectedIndex = _selectedTabIndex;
        switcher.SelectedIndex = _selectedTabIndex;

        Content = new Grid
        {
            Children =
            {
                switcher,
                tabHostView
            }
        };
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