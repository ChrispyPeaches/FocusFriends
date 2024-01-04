using FocusApp.Resources;

namespace FocusApp;

class AppShell : Shell
{
    public AppShell()
    {
        TabBar tabBar = new TabBar();
        tabBar.Style = AppStyles.ShellStyle;

        Tab tabShop = new Tab { Title = "Shop", /*Icon = "home.png"*/};
        tabShop.Items.Add(
            new ShellContent 
            { 
                Content = new Pages.Shop.MainPage()
            });

        Tab tabTimer = new Tab { Title = "Timer", /*Icon = "home.png"*/ };
        tabTimer.Items.Add(
            new ShellContent
            {
                Content = new Pages.TimerPage()
            });

        Tab tabSocial = new Tab { Title = "Social", /*Icon = "home.png"*/ };
        tabSocial.Items.Add(
            new ShellContent
            {
                Content = new Pages.Social.MainPage() 
            });

        tabBar.Items.Add(tabShop);
        tabBar.Items.Add(tabTimer);
        tabBar.Items.Add(tabSocial);

        Items.Add(tabBar);
    }
}