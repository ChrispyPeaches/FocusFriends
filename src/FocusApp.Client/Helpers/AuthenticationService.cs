using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using FocusApp.Shared.Models;

namespace FocusApp.Client.Helpers;

internal interface IAuthenticationService
{
    static AuthenticationService Instance { get; }
    string Id { get; set; }
    string Email { get; set; }
    string AuthToken { get; set; }
    User? CurrentUser { get; set; }
    Island? SelectedIsland { get; set; }
    Pet? SelectedPet { get; set; }
    Badge? SelectedBadge { get; set; }
    Furniture? SelectedFurniture { get; set; }

    event PropertyChangedEventHandler? PropertyChanged;
}

public partial class AuthenticationService : INotifyPropertyChanged, IAuthenticationService
{
    private static AuthenticationService? _instance;
    public static AuthenticationService Instance => _instance ??= new AuthenticationService();

    public event PropertyChangedEventHandler? PropertyChanged;
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string AuthToken { get; set; } = "";

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

    private Furniture? _selectedFurniture;
    public Furniture? SelectedFurniture
    {
        get => _selectedFurniture;
        set => SetProperty(ref _selectedFurniture, value);
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

partial class ObservableUser : ObservableObject
{
    [ObservableProperty]
    private User? _user;
    public ObservableUser(User? user) => this._user = user;

    public Island? SelectedIsland
    {
        get => _user.SelectedIsland;
        set => SetProperty(_user.SelectedIsland, value, _user, (u, n) => u.SelectedIsland = n);
    }

    public Pet? SelectedPet
    {
        get => _user.SelectedPet;
        set => SetProperty(_user.SelectedPet, value, _user, (u, n) => u.SelectedPet = n);
    }

    public Badge? SelectedBadge
    {
        get => _user.SelectedBadge;
        set => SetProperty(_user.SelectedBadge, value, _user, (u, n) => u.SelectedBadge = n);
    }

    public Furniture? SelectedFurniture
    {
        get => _user.SelectedFurniture;
        set => SetProperty(_user.SelectedFurniture, value, _user, (u, n) => u.SelectedFurniture = n);
    }
}