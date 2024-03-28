using CommunityToolkit.Maui.Markup;
using FocusApp.Shared.Models;
using Maui.FreakyControls;

namespace FocusApp.Client.Views.Controls;

internal class IslandDisplayView : Grid
{
    #region Bindable Properties

    private Island _island;
    public Island Island
    {
        get => (Island)GetValue(IslandProperty);
        set => SetValue(IslandProperty, value);
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
        get => (Pet)GetValue(PetProperty);
        set => SetValue(PetProperty, value);
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


    public IslandDisplayView()
    {
        RowDefinitions = GridRowsColumns.Rows.Define(
            (Row.Island, GridRowsColumns.Stars(3)),
            (Row.PetAndIsland, GridRowsColumns.Stars(1))
        );
        ColumnDefinitions = GridRowsColumns.Columns.Define(
            (Column.LeftTimerButton, GridRowsColumns.Stars(1)),
            (Column.TimerAmount, GridRowsColumns.Stars(2)),
            (Column.RightTimerButton, GridRowsColumns.Stars(1))
        );

        // Island
        Children.Add(
            new FreakySvgImageView()
            {
                ImageColor = Colors.Transparent,
                SvgMode = Aspect.AspectFit
            }
            .Bind(
                FreakySvgImageView.Base64StringProperty,
                getter: static (view) => view.Island,
                convert: (island) => Convert.ToBase64String(island.Image),
                source: this)
            .Row(Row.Island)
            .RowSpan(2)
            .ColumnSpan(typeof(Column).GetEnumNames().Length)
            .Margins(left: 10, right: 10));


        // Pet
        Children.Add(
            new FreakySvgImageView()
            {
                ImageColor = Colors.Transparent,
                HeightRequest = 90
            }
            .Bind(
                FreakySvgImageView.Base64StringProperty,
                getter: static (view) => view.Pet,
                convert: (pet) => Convert.ToBase64String(pet.Image),
                source: this)
            .Row(Row.PetAndIsland)
            .Column(Column.RightTimerButton)
            .Margins(bottom: 60)
            .Bottom()
            .End()
            );
    }
}