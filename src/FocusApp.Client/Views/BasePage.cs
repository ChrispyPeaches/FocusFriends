using System.ComponentModel;
using System.Runtime.CompilerServices;
using SimpleToolkit.SimpleShell.Extensions;

namespace FocusApp.Client.Views;

/// <summary>
/// Used for simplifying dependency injection for pages.
/// </summary>
internal class BasePage : ContentPage, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    internal bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    protected async void BackButtonClicked(object sender, EventArgs e)
    {
        // Back navigation reverses animation so can keep right to left
        Shell.Current.SetTransition(Transitions.RightToLeftPlatformTransition);
        await Shell.Current.GoToAsync("..");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await AppShell.Current.SetTabBarIsVisible(true);
    }

    #region Property Changed Notification Logic

    private void SetProperty<T>(ref T backingStore, in T value, [CallerMemberName] in string propertyname = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return;
        }

        backingStore = value;

        OnPropertyChanged(propertyname);
    }

    void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    #endregion
}

