using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Markup;
using FocusApp.Shared.Models;
using Maui.FreakyControls;

namespace FocusApp.Client.Views.Controls;

internal class IslandDisplayView : ContentView, INotifyPropertyChanged
{
    #region Bindable Properties

    private Island _island;
    public Island Island
    {
        get => _island;
        set => SetProperty(ref _island, value);
    }


    /// <summary>Bindable property for <see cref="Island"/>.</summary>
    public static readonly BindableProperty IslandProperty = BindableProperty.Create(
        propertyName: nameof(Island),
        returnType: typeof(Island),
        declaringType: typeof(IslandDisplayView),
        validateValue: (_, value) => value != null);

    private Pet _pet;
    public Pet Pet
    {
        get => _pet;
        set => SetProperty(ref _pet, value);
    }

    /// <summary>Bindable property for <see cref="Pet"/>.</summary>
    public static readonly BindableProperty PetProperty = BindableProperty.Create(
        propertyName: nameof(Pet),
        returnType: typeof(Pet),
        declaringType: typeof(IslandDisplayView),
        validateValue: (_, value) => value != null);

    #endregion

    enum Row { Island, PetAndIsland}
    enum Column { LeftTimerButton, TimerAmount, RightTimerButton }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IslandDisplayView()
    {
        Content = new Grid()
        {
            RowDefinitions = GridRowsColumns.Rows.Define(
                (Row.Island, GridRowsColumns.Stars(3)),
                (Row.PetAndIsland, GridRowsColumns.Stars(1))
            ),
            ColumnDefinitions = GridRowsColumns.Columns.Define(
                (Column.LeftTimerButton, GridRowsColumns.Stars(1)),
                (Column.TimerAmount, GridRowsColumns.Stars(2)),
                (Column.RightTimerButton, GridRowsColumns.Stars(1))
            ),

            Children =
            {
                // Island
                new FreakySvgImageView()
                {
                    ImageColor = Colors.Transparent,
                    SvgMode = Aspect.AspectFit
                }
                .Bind(
                    FreakySvgImageView.Base64StringProperty,
                    getter:  static (view) => view.Island,
                    convert: (island) => Convert.ToBase64String(island.Image),
                    source: this)
                .Row(Row.Island)
                .RowSpan(2)
                .ColumnSpan(typeof(Column).GetEnumNames().Length)
                .Margins(left: 10, right: 10),

                // Pet
                new FreakySvgImageView()
                {
                    ImageColor = Colors.Transparent,
                    HeightRequest = 90
                }
                .Bind(
                    FreakySvgImageView.Base64StringProperty,
                    getter:  static (view) => view.Pet,
                    convert: (pet) => Convert.ToBase64String(pet.Image),
                    source: this)
                .Row(Row.PetAndIsland)
                .Column(Column.TimerAmount)
                .Margins(bottom: 60)
                .Bottom()
                .End(),
            }
        };
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