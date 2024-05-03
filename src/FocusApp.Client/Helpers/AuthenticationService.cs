using System.ComponentModel;
using System.Runtime.CompilerServices;
using Auth0.OidcClient;
using FocusApp.Client.Views;
using FocusApp.Shared.Models;

namespace FocusApp.Client.Helpers;

internal interface IAuthenticationService
{
    string? Auth0Id { get; set; }
    string? Email { get; set; }
    User? CurrentUser { get; set; }
    Island? SelectedIsland { get; set; }
    Pet? SelectedPet { get; set; }
    Badge? SelectedBadge { get; set; }
    Decor? SelectedDecor { get; set; }
    int Balance { get; set; }

    event PropertyChangedEventHandler? PropertyChanged;

    void ClearUser();
    Task Logout(IAuth0Client auth0Client);
    void PopulateWithUserData(User user);
}

public class AuthenticationService : INotifyPropertyChanged, IAuthenticationService
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public string? Auth0Id { get; set; } = "";
    public string? Email { get; set; } = "";

    private User? _currentUser;
    public User? CurrentUser
    {
        get => _currentUser;
        set => SetProperty(ref _currentUser, value);
    }

    private int? _balance;
    public int Balance
    {
        get => _balance ?? 0;
        set => SetProperty(ref _balance, value);
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

    /// <summary>
    /// Remove user data from auth service and secure storage,
    /// navigate to the login page,and
    /// log the user out using the auth0 client.
    /// </summary>
    /// <remarks>
    /// Note that the auth0Client.LogoutAsync needs to happen after the navigation
    /// to LoginPage due to an issue with Maui navigation handlers.
    /// Details here: https://github.com/dotnet/maui/issues/11259
    /// </remarks>
    public async Task Logout(IAuth0Client auth0Client)
    {
        ClearUser();

        SecureStorage.Default.Remove("id_token");
        SecureStorage.Default.Remove("access_token");

        await Shell.Current.GoToAsync("///" + nameof(LoginPage));

        await auth0Client.LogoutAsync();
    }

    /// <summary>
    /// Remove all data for the "logged in" user.
    /// </summary>
    public void ClearUser()
    {
        Auth0Id = string.Empty;
        Email = string.Empty;
        Balance = 0;
        CurrentUser = null;
        SelectedIsland = null;
        SelectedPet = null;
        SelectedBadge = null;
        SelectedDecor = null;
    }

    public void PopulateWithUserData(User user)
    {
        CurrentUser = user;

        Auth0Id = user.Auth0Id;
        Email = user.Email;
        Balance = user.Balance;
        SelectedBadge = user.SelectedBadge;
        SelectedDecor = user.SelectedDecor;
        SelectedIsland = user.SelectedIsland;
        SelectedPet = user.SelectedPet;
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