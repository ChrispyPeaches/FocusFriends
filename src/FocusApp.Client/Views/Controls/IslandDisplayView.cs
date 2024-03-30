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

    public Furniture Decor
    {
        get => (Furniture)GetValue(DecorProperty);
        set => SetValue(DecorProperty, value);
    }

    /// <summary>Bindable property for <see cref="Decor"/>.</summary>
    public static readonly BindableProperty DecorProperty = BindableProperty.Create(
        propertyName: nameof(Decor),
        returnType: typeof(Furniture),
        declaringType: typeof(IslandDisplayView),
        validateValue: (_, value) => value != null);

    #endregion

    enum Row {PetAndDecor, BottomSpacer}
    enum Column {LeftWhiteSpace, Decor, Pet, RightWhiteSpace }

    public IslandDisplayView()
    {
        RowDefinitions = GridRowsColumns.Rows.Define(
            (Row.PetAndDecor, GridRowsColumns.Stars(7)),
            (Row.BottomSpacer, GridRowsColumns.Stars(3))
        );
        ColumnDefinitions = GridRowsColumns.Columns.Define(
            (Column.LeftWhiteSpace, GridRowsColumns.Stars(2)),
            (Column.Decor, GridRowsColumns.Stars(6)),
            (Column.Pet, GridRowsColumns.Stars(6)),
            (Column.RightWhiteSpace, GridRowsColumns.Stars(2))
        );

        Children.Add(GetIslandView());
        Children.Add(GetDecorView());
        Children.Add(GetPetView());
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

    /// <summary>
    /// Create a container specifying an area which a pet can occupy
    /// and place the pet in the bottom right corner of that container.
    /// </summary>
    public View GetPetView()
    {
        FlexLayout petContainer = new FlexLayout()
            {
                JustifyContent = FlexJustify.End,
                Direction = FlexDirection.Column
            }
            .Row(Row.PetAndDecor)
            .Column(Column.Pet);

        petContainer.Children
            .Add(new Image()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    MaximumWidthRequest = petContainer.Width,
                    MaximumHeightRequest = petContainer.Height
            }
                .Bind(
                    Image.SourceProperty,
                    getter: static (view) => view.Pet,
                    convert: (pet) => new ByteArrayToImageSourceConverter().ConvertFrom(pet?.Image),
                    source: this)
                .Bind(
                    Image.HeightRequestProperty,
                    getter: static (view) => view.Pet,
                    convert: (pet) => pet?.HeightRequest,
                    source: this)
                .Bind(
                    Image.MaximumHeightRequestProperty,
                    getter: (FlexLayout container) => container.Height,
                    source: petContainer)
                .Bind(
                    Image.MaximumWidthRequestProperty,
                    getter: (FlexLayout container) => container.Width,
                    source: petContainer)
            );

        return petContainer;
    }

    /// <summary>
    /// Create a container specifying an area which a decor item can occupy
    /// and place the decor item in the bottom left corner of that container.
    /// </summary>
    public View GetDecorView()
    {
        FlexLayout decorContainer = new FlexLayout()
            {
                JustifyContent = FlexJustify.End,
                Direction = FlexDirection.Column
        }
            .Row(Row.PetAndDecor)
            .Column(Column.Decor);

        decorContainer.Children
            .Add(new Image()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    MaximumWidthRequest = decorContainer.Width,
                    MaximumHeightRequest = decorContainer.Height
                }
                .Bind(
                    Image.SourceProperty,
                    getter: static (view) => view.Decor,
                    convert: (decor) => new ByteArrayToImageSourceConverter().ConvertFrom(decor?.Image),
                    source: this)
                .Bind(
                    Image.HeightRequestProperty,
                    getter: static (view) => view.Decor,
                    convert: (decor) => decor?.HeightRequest,
                    source: this)
                .Bind(
                    Image.MaximumHeightRequestProperty,
                    getter: (FlexLayout container) => container.Height,
                    source: decorContainer)
                .Bind(
                    Image.MaximumWidthRequestProperty,
                    getter: (FlexLayout container) => container.Width,
                    source: decorContainer)
            );

        return decorContainer;
    }

}