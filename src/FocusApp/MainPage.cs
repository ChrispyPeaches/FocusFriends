using CommunityToolkit.Maui.Markup;
using FocusApp.Views;
using Sharpnado.Tabs;
using FocusApp.Helpers;

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

    public void Build()
    {
        TabHostView tabHostView = new TabHostView
        {
            WidthRequest = 250,
            HeightRequest = 60,
            CornerRadius = 30,
            BackgroundColor = Color.FromArgb("#7B7B7B"),
            IsSegmented = true,
            Orientation = OrientationType.Horizontal,
            SegmentedOutlineColor = Color.FromArgb("#B4B4B4"),
            TabType = TabType.Fixed,
            VerticalOptions = LayoutOptions.End,
            Tabs =
            {
                new BottomTabItem()
                {
                    Label = "Shop"
                },
                new BottomTabItem()
                {
                    Label = "Timer"
                },
                new BottomTabItem()
                {
                    Label = "Social"
                }
            }
        }
        .Paddings(20, 0)
        .CenterHorizontal();

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

    private void ReloadUI(Type[] obj)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Build();
        });
    }
}