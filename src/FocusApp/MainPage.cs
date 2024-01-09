using CommunityToolkit.Maui.Markup;
using FocusApp.Views;
using Sharpnado.Tabs;
using FocusApp.Helpers;
using FocusApp.Resources.FontAwesomeIcons;
using FocusApp.Resources;

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

    public BottomTabItem GenerateShopTab()
    {
        


        BottomTabItem shopTab = new BottomTabItem()
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,  
        }
        .Paddings(
            top: 5,
            bottom: 5,
            left: 10,
            right: 10);

        Frame frame = new Frame()
        {
            CornerRadius = 20,
            WidthRequest = 100,
            HeightRequest = 40,
            HasShadow = false,
            BackgroundColor = AppStyles.navigationBarButtonBackgroundColor,
            Padding = 0,
            Content = new Label()
            {
                Text = SolidIcons.BagShopping,
                FontFamily = nameof(SolidIcons),
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        }.TapGesture(() =>
        {
            shopTab.
        });

        shopTab.Content = frame;

        return shopTab;
    }

    public BottomTabItem GenerateTimerTab()
    {
        return new BottomTabItem()
        {
            Content = new Frame()
            {
                Content = new Label()
                {
                    Text = SolidIcons.Clock,
                    FontFamily = nameof(SolidIcons),
                    BackgroundColor = AppStyles.navigationBarButtonBackgroundColor,
                    FontSize = 10,
                }
            }
        };
    }

    public void Build()
    {
        TabHostView tabHostView = new TabHostView
        {
            WidthRequest = 400,
            HeightRequest = 60,
            IsSegmented = true,
            Orientation = OrientationType.Horizontal,
            SegmentedOutlineColor = Color.FromArgb("#B4B4B4"),
            TabType = TabType.Fixed,
            VerticalOptions = LayoutOptions.End,
            HorizontalOptions = LayoutOptions.Center,
            Tabs =
            {
                // Shop Tab
                GenerateShopTab(),

                // Timer Tab
                new BottomTabItem()
                {
                    Label = SolidIcons.Clock,
                    FontFamily = nameof(SolidIcons)
                },

                // Social Tab
                new BottomTabItem()
                {
                    Label = SolidIcons.Users,
                    FontFamily = nameof(SolidIcons)
                }
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
    /// Used to allow hot reload to work
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