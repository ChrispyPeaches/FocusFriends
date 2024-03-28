using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using FocusApp.Shared.Models;

namespace FocusApp.Client.Helpers;

internal interface IAuthenticationService
{
    string Id { get; set; }
    string Email { get; set; }
    string AuthToken { get; set; }
    User? CurrentUser { get; set; }
    Island? SelectedIsland { get; }
    Pet? SelectedPet { get; }
    Badge? SelectedBadge { get; }
    Furniture? SelectedFurniture { get; }
}

public partial class AuthenticationService : ObservableObject, IAuthenticationService
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string AuthToken { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedIsland))]
    [NotifyPropertyChangedFor(nameof(SelectedPet))]
    [NotifyPropertyChangedFor(nameof(SelectedBadge))]
    [NotifyPropertyChangedFor(nameof(SelectedFurniture))]
    private User? _currentUser;

    public Island? SelectedIsland => CurrentUser?.SelectedIsland;
    public Pet? SelectedPet => CurrentUser?.SelectedPet;
    public Badge? SelectedBadge => CurrentUser?.SelectedBadge;
    public Furniture? SelectedFurniture => CurrentUser?.SelectedFurniture;
}