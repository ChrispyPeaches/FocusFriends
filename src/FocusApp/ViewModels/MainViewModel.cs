using CommunityToolkit.Mvvm.ComponentModel;
using FocusApp.ViewModels;
using System.Diagnostics;

namespace FocusApp
{
    partial class MainViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string? name;

        partial void OnNameChanging(string? value)
        {
            Debug.WriteLine($"Name is about to change to {value}");
        }

        partial void OnNameChanged(string? value)
        {
            Debug.WriteLine($"Name has changed to {value}");
        }
    }
}
