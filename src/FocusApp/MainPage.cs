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

    /// <summary>
    /// Generate a tab with the specified icon.
    /// </summary>
    /// <param name="tabIcon">A FontIcon constant (From <see cref="FocusApp.Resources.FontAwesomeIcons"/>)</param>
    /// <returns></returns>
    private static BottomTabItem GenerateTab(string tabIcon)
    {
        Frame frame = new Frame()
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
                Text = tabIcon
            }
        }
        .FillVertical()
        .FillHorizontal();

        BottomTabItem baseTab = new BottomTabItem()
        {
            Background = Colors.Transparent,
            BackgroundColor = Colors.Transparent,
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

    /// <summary>
    /// For the main page, the UI has a separate method so it can be rebuilt when Hot Reload is triggered.
    /// </summary>
    public void Build()
    {
        TabHostView tabHostView = new TabHostView
        {
            HeightRequest = 70,
            IsSegmented = true, SegmentedHasSeparator = false, SegmentedOutlineColor = Colors.Transparent,
            Orientation = OrientationType.Horizontal,
            TabType = TabType.Fixed,
            VerticalOptions = LayoutOptions.End,
            BackgroundColor = Colors.Transparent,
            Tabs =
            {
                GenerateTab(SolidIcons.BagShopping)
                    .Paddings(top: 10, bottom: 10, left: 15, right: 15),
                GenerateTab(SolidIcons.Clock)
                    .Paddings(top: 10, bottom: 10, left: 30, right: 30),
                GenerateTab(SolidIcons.Users)
                    .Paddings(top: 10, bottom: 10, left: 15, right: 15),
            }
        }
        .Paddings(0, 0)
        .FillHorizontal();

        VisualTreeElementExtensions
            .GetVisualTreeDescendants(tabHostView)
            .OfType<Grid>()
            .First()
            .ColumnDefinitions = GridRowsColumns.Columns.Define(
                GridRowsColumns.Stars(1),
                GridRowsColumns.Stars(2),
                GridRowsColumns.Stars(1));

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