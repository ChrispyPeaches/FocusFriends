using System.ComponentModel;
using System.Runtime.CompilerServices;
using Auth0.OidcClient;
using FocusApp.Client.Views;
using FocusApp.Shared.Models;

namespace FocusApp.Client.Helpers;

internal interface IAuthenticationService
{
    bool IsLoggedIn { get; }
    Guid? Id { get; set; }
    string? Auth0Id { get; set; }
    string? Email { get; set; }
    string? UserName { get; set; }
    string? Pronouns { get; set; }
    int Balance { get; set; }
    DateTime? DateCreated { get; set; }
    byte[]? ProfilePicture { get; set; }
    Island? SelectedIsland { get; set; }
    Pet? SelectedPet { get; set; }
    Badge? SelectedBadge { get; set; }
    Decor? SelectedDecor { get; set; }
    Task? StartupSyncTask { get; set; }

    event PropertyChangedEventHandler? PropertyChanged;
    void ClearUser();
    Task Logout(IAuth0Client auth0Client);
    void PopulateWithUserData(User user);
}

public class AuthenticationService : INotifyPropertyChanged, IAuthenticationService
{
    public bool IsLoggedIn => Auth0Id is not null;
    public event PropertyChangedEventHandler? PropertyChanged;
    public Guid? Id { get; set; }
    public string? Auth0Id { get; set; } = "";
    public string? Email { get; set; } = "";
    public string? UserName { get; set; } = "";
    public string? Pronouns { get; set; } = "";
    public DateTime? DateCreated { get; set; }
    public byte[]? ProfilePicture { get; set; }
    public Task? StartupSyncTask { get; set; }

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
        Id = null;
        Auth0Id = null;
        Email = null;
        UserName = null;
        Pronouns = null;
        Balance = 0;
        DateCreated = DateTime.MinValue;
        ProfilePicture = null;
        SelectedIsland = null;
        SelectedBadge = null;
        SelectedDecor = null;
        SelectedPet = null;
    }

    public void PopulateWithUserData(User user)
    {
        Id = user.Id;
        Auth0Id = user.Auth0Id;
        Email = user.Email;
        UserName = user.UserName;
        Pronouns = user.Pronouns;
        Balance = user.Balance;
        DateCreated = user.DateCreated;
        ProfilePicture = user.ProfilePicture;
        SelectedIsland = user.SelectedIsland;
        SelectedBadge = user.SelectedBadge;
        SelectedDecor = user.SelectedDecor;
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