using System.ComponentModel;
using System.Runtime.CompilerServices;
using FocusApp.Shared.Models;

namespace FocusApp.Client.Helpers;

internal interface IAuthenticationService
{
    string? Auth0Id { get; set; }
    string? Email { get; set; }
    string? AuthToken { get; set; }
    User? CurrentUser { get; set; }
    Island? SelectedIsland { get; set; }
    Pet? SelectedPet { get; set; }
    Badge? SelectedBadge { get; set; }
    Decor? SelectedDecor { get; set; }

    event PropertyChangedEventHandler? PropertyChanged;
}

public class AuthenticationService : INotifyPropertyChanged, IAuthenticationService
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public string? Auth0Id { get; set; } = "";
    public string? Email { get; set; } = "";
    public string? AuthToken { get; set; } = "";

    private User? _currentUser;
    public User? CurrentUser
    {
        get => _currentUser;
        set => SetProperty(ref _currentUser, value);
    }

    private Island? _selectedIsland;
    public Island? SelectedIsland
    {
        get => _selectedIsland;
        set => SetProperty(ref _selectedIsland, value);
    }

    private Pet? _selectedPet;
    public Pet? SelectedPet
    {
        get => _selectedPet;
        set => SetProperty(ref _selectedPet, value);
    }

    private Badge? _selectedBadge;
    public Badge? SelectedBadge
    {
        get => _selectedBadge;
        set => SetProperty(ref _selectedBadge, value);
    }

    private Decor? _selectedDecor;
    public Decor? SelectedDecor
    {
        get => _selectedDecor;
        set => SetProperty(ref _selectedDecor, value);
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