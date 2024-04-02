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
        declaringType: typeof(IslandDisplayView));

    public Pet Pet
    {
        get => (Pet)GetValue(PetProperty);
        set => SetValue(PetProperty, value);
    }

    /// <summary>Bindable property for <see cref="Pet"/>.</summary>
    public static readonly BindableProperty PetProperty = BindableProperty.Create(
        propertyName: nameof(Pet),
        returnType: typeof(Pet),
        declaringType: typeof(IslandDisplayView));

    public Furniture Decor
    {
        get => (Furniture)GetValue(DecorProperty);
        set => SetValue(DecorProperty, value);
    }

    /// <summary>Bindable property for <see cref="Decor"/>.</summary>
    public static readonly BindableProperty DecorProperty = BindableProperty.Create(
        propertyName: nameof(Decor),
        returnType: typeof(Furniture),
        declaringType: typeof(IslandDisplayView));

    #endregion

    enum Column {LeftWhiteSpace, PetAndDecor, RightWhiteSpace }

    public IslandDisplayView()
    {
        RowDefinitions = GridRowsColumns.Rows.Define(
            GridRowsColumns.Auto
        );
        ColumnDefinitions = GridRowsColumns.Columns.Define(
            (Column.LeftWhiteSpace, GridRowsColumns.Stars(1)),
            (Column.PetAndDecor, GridRowsColumns.Stars(6)),
            (Column.RightWhiteSpace, GridRowsColumns.Stars(1))
        );

        Image islandView = GetIslandView();

        Children.Add(islandView);
        Children.Add(GetPetAndDecorLayout(islandView));
    }

    public Image GetIslandView() =>
        new Image()
            {
                ZIndex = 0
            }
            .Bind(
                Image.SourceProperty,
                getter: static (view) => view.Island,
                convert: static (island) => new ByteArrayToImageSourceConverter().ConvertFrom(island?.Image),
                source: this)
            .ColumnSpan(typeof(Column).GetEnumNames().Length)
            .Margins(left: 10, right: 10);

    /// <summary>
    /// Create a layout for vertical spacing and a container for horizontal distribution
    /// with the pet and decor items displayed inside.
    /// </summary>
    /// <param name="islandView">The island view which will be used to determine the height of the layout.</param>
    /// <returns>The layout.</returns>
    public FlexLayout GetPetAndDecorLayout(Image islandView)
    {
        FlexLayout petAndDecorLayout = new FlexLayout()
            {
                Direction = FlexDirection.Column,
                JustifyContent = FlexJustify.Start,
                MaximumHeightRequest = islandView.Height,
                ZIndex = 1
            }
            .Bind(
                FlexLayout.MaximumHeightRequestProperty,
                getter: static (Image island) => island.Height,
                source: islandView)
            .Row(0)
            .Column(Column.PetAndDecor);

        {
            FlexLayout petAndDecorContainer = new FlexLayout()
                {
                    Direction = FlexDirection.Row
                }
                .Basis(new FlexBasis(0.8f, true));
            petAndDecorContainer.Children.Add(GetDecorView());
            petAndDecorContainer.Children.Add(GetPetView());

            petAndDecorLayout.Children.Add(petAndDecorContainer);
        }

        return petAndDecorLayout;
    }

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
            .Basis(new FlexBasis(0.5f, true));

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
                    convert: static (pet) => new ByteArrayToImageSourceConverter().ConvertFrom(pet?.Image),
                    source: this)
                .Bind(
                    Image.HeightRequestProperty,
                    getter: static (view) => view.Pet,
                    convert: static (pet) => pet?.HeightRequest,
                    source: this)
                .Bind(
                    Image.MaximumHeightRequestProperty,
                    getter: static (FlexLayout container) => container.Height,
                    source: petContainer)
                .Bind(
                    Image.MaximumWidthRequestProperty,
                    getter: static (FlexLayout container) => container.Width,
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
            .Basis(new FlexBasis(0.5f, true));

        decorContainer.Children
            .Add(new Image()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    MaximumWidthRequest = decorContainer.Width,
                    MaximumHeightRequest = decorContainer.Height,

                }
                .Bind(
                    Image.SourceProperty,
                    getter: static (view) => view.Decor,
                    convert: static (decor) => new ByteArrayToImageSourceConverter().ConvertFrom(decor?.Image),
                    source: this)
                .Bind(
                    Image.HeightRequestProperty,
                    getter: static (view) => view.Decor,
                    convert: static (decor) => decor?.HeightRequest,
                    source: this)
                .Bind(
                    Image.MaximumHeightRequestProperty,
                    getter: static (FlexLayout container) => container.Height,
                    source: decorContainer)
                .Bind(
                    Image.MaximumWidthRequestProperty,
                    getter: static (FlexLayout container) => container.Width,
                    source: decorContainer)
            );

        return decorContainer;
    }
}