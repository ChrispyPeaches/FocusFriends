using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using FocusApp.Shared.Models;
using Microsoft.Maui.Layouts;

namespace FocusApp.Client.Views.Controls;

internal class IslandDisplayView : Grid
{
    #region Bindable Properties

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

    enum Row {Pet, BottomSpacer}
    enum Column { Pet, RightWhiteSpace }

    public IslandDisplayView()
    {
        RowDefinitions = GridRowsColumns.Rows.Define(
            (Row.Pet, GridRowsColumns.Stars(6)),
            (Row.BottomSpacer, GridRowsColumns.Stars(3))
        );
        ColumnDefinitions = GridRowsColumns.Columns.Define(
            (Column.Pet, GridRowsColumns.Stars(5)),
            (Column.RightWhiteSpace, GridRowsColumns.Stars(1))
        );

        Children.Add(GetIslandView());
        Children.Add(GetPetView());
    }

    /// <summary>
    /// Create a container specifying an area which a pet can occupy
    /// and place the pet in the bottom right corner of that container.
    /// </summary>
    public View GetPetView()
    {
        var petContainer = new FlexLayout()
            {
                BackgroundColor = Color.FromRgba(0, 255, 255, 0.8),
                JustifyContent = FlexJustify.End,
                Direction = FlexDirection.Column
            }
            .Row(Row.Pet)
            .Column(Column.Pet);

        petContainer.Children
            .Add(new Image()
                {
                    BackgroundColor = Color.FromRgba(0, 255, 0, 0.3),
                    HeightRequest = 90
                }
                .AlignSelf(FlexAlignSelf.End)
                .Bind(
                    Image.SourceProperty,
                    getter: static (view) => view.Pet,
                    convert: (pet) => new ByteArrayToImageSourceConverter().ConvertFrom(pet?.Image),
                    source: this)
                .Bind(
                    Image.HeightRequestProperty,
                    getter: static (view) => view.Pet,
                    convert: (pet) => pet.HeightRequest,
                    source: this)
            );

        return petContainer;
    }

    public View GetIslandView() =>
        new Image()
            .Bind(
                Image.SourceProperty,
                getter: static (view) => view.Island,
                convert: static (island) => new ByteArrayToImageSourceConverter().ConvertFrom(island?.Image),
                source: this)
            .RowSpan(typeof(Row).GetEnumNames().Length)
            .ColumnSpan(typeof(Column).GetEnumNames().Length)
            .Margins(left: 10, right: 10);
}