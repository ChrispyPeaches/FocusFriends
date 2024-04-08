using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Views.Controls;
using FocusApp.Shared.Models;
using FocusCore.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Layouts;

namespace FocusApp.Client.Views.Social;

[QueryProperty(nameof(DisplayUser), nameof(DisplayUser))]
internal class FriendProfilePage : BasePage, IQueryAttributable
{
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        object id = null;
        if (query.TryGetValue(nameof(DisplayUserAuth0Id), out id))
            DisplayUserAuth0Id = id as string;
    }

    #region Bindable Properties

    public User DisplayUser
    {
        get => (User)GetValue(DisplayUserProperty);
        set => SetValue(DisplayUserProperty, value);
    }

    /// <summary>Bindable property for <see cref="DisplayUser"/>.</summary>
    public static readonly BindableProperty DisplayUserProperty = BindableProperty.Create(
        propertyName: nameof(DisplayUser),
        returnType: typeof(User),
        declaringType: typeof(FriendProfilePage));

    public string DisplayUserAuth0Id
    {
        get => (string)GetValue(DisplayUserAuth0IdProperty);
        set
        {
            SetValue(DisplayUserAuth0IdProperty, value);
            DisplayUserIdSet.Invoke();
        }
    }


    /// <summary>Bindable property for <see cref="DisplayUserAuth0Id"/>.</summary>
    public static readonly BindableProperty DisplayUserAuth0IdProperty = BindableProperty.Create(
        propertyName: nameof(DisplayUser),
        returnType: typeof(string),
        declaringType: typeof(FriendProfilePage));

    #endregion

    public static Dictionary<string, object> BuildParamterArgs(string displayUserId)
    {
        return new Dictionary<string, object>
        {
            { nameof(DisplayUserAuth0Id), displayUserId}
        };
    }

    private Action DisplayUserIdSet;

    enum Column { ProfilePicture, ProfileInfo }
    enum Row { TopBar, ProfileInfo, IslandView, BottomWhiteSpace }

    private readonly IMediator _mediator;
    private readonly ILogger<FriendProfilePage> _logger;

    public Grid GetBaseGrid()
    {
        return new Grid()
        {
            RowDefinitions = GridRowsColumns.Rows.Define(
                (Row.TopBar, 80),
                (Row.ProfileInfo, GridRowsColumns.Stars(4)),
                (Row.IslandView, GridRowsColumns.Stars(4)),
                (Row.BottomWhiteSpace, GridRowsColumns.Stars(2))
            ),
            ColumnDefinitions = GridRowsColumns.Columns.Define(
                (Column.ProfilePicture, GridRowsColumns.Stars(2)),
                (Column.ProfileInfo, GridRowsColumns.Stars(2))
            ),
            Children =
            {
                new ActivityIndicator()
                    {
                        Color = AppStyles.Palette.OrchidPink
                    }
                    .ZIndex(2)
                    .Row(Row.ProfileInfo, Row.BottomWhiteSpace)
                    .Column(typeof(Column).GetEnumNames().Length)
                    .Center()
                    .Bind(
                        ActivityIndicator.IsRunningProperty,
                        getter: (page) => page.IsBusy,
                        source: this)
            }
        };
    }

    public FriendProfilePage(
        IMediator mediator,
        ILogger<FriendProfilePage> logger)
    {
        _mediator = mediator;
        _logger = logger;

        DisplayUserIdSet += OnIdSet;

        BackgroundColor = AppStyles.Palette.LightPeriwinkle;

        Content = GetBaseGrid();
    }

    public void BuildUI()
    {
        var grid = GetBaseGrid();
        grid.Children.Add(
            GetTopBar()
                .Row(Row.TopBar)
                .ColumnSpan(typeof(Column).GetEnumNames().Length));
        grid.Children.Add(
            GetProfileInfo()
                .Row(Row.ProfileInfo)
                .Column(Column.ProfileInfo));
        grid.Children.Add(
            GetIslandView()
                .CenterVertical()
                .Row(Row.IslandView)
                .ColumnSpan(typeof(Column).GetEnumNames().Length));

        MainThread.BeginInvokeOnMainThread(() => Content = grid);
    }

    public View GetTopBar()
    {
        return new FlexLayout()
        {
            JustifyContent = FlexJustify.Start,

            Children =
            {
                // Back Button
                new Button
                    {
                        Text = SolidIcons.ChevronLeft,
                        TextColor = Colors.Black,
                        FontFamily = nameof(SolidIcons),
                        FontSize = 40,
                        BackgroundColor = Colors.Transparent
                    }
                    .Paddings(top: 10, bottom: 10, left: 15, right: 15)
                    // When clicked, go to timer view
                    .Invoke(button => button.Released += (sender, eventArgs) =>
                        BackButtonClicked(sender, eventArgs)),

                // Header
                new Label
                    {
                        TextColor = Colors.Black,
                        FontSize = 25
                    }
                    .CenterVertical()
                    .Paddings(left: 5, right: 5)
                    .Bind(
                        Label.TextProperty,
                        getter: (page) => page.DisplayUser,
                        convert: (user) => $"{user?.UserName}'s Profile" ,
                        source: this),
            }
        };

    }

    public FlexLayout GetProfileInfo()
    {
        var a = new FlexLayout()
        {
            Direction = FlexDirection.Column,
            AlignItems = FlexAlignItems.End,
            JustifyContent = FlexJustify.Start,
        };

        a.Children.Add(
            new Label()
                {
                    FontSize = 20
                }
                .Padding(15, 2)
                .Bind(
                    Label.TextProperty,
                    getter: (page) => page.DisplayUser,
                    convert: (user) => user?.UserName,
                    source: this));
        a.Children.Add(
            new Label()
            {
                FontSize = 20
            }
            .Padding(15, 2)
            .Bind(
                Label.TextProperty,
                getter: (page) => page.DisplayUser,
                convert: (user) => user?.FullName(),
                source: this));
        a.Children.Add(
            new Label()
                {
                    FontSize = 20
            }
                .Paddings(right: 15, top: 2, bottom: 12)
                .Bind(
                    Label.TextProperty,
                    getter: (page) => page.DisplayUser,
                    convert: (user) => user?.Pronouns,
                    source: this));
        a.Children.Add(
            new Frame()
            {
                HeightRequest = 80,
                WidthRequest = HeightRequest,
                BackgroundColor = Colors.PaleVioletRed,
                Padding = 5,
                Content = new Image()
                {
                    Aspect = Aspect.AspectFit,
                    Source = new ByteArrayToImageSourceConverter().ConvertFrom(DisplayUser?.SelectedBadge?.Image)
                }
                .Grow(1)
                .Bind(
                    Image.SourceProperty,
                    getter: (page) => page.DisplayUser,
                    convert: (user) => new ByteArrayToImageSourceConverter().ConvertFrom(user?.SelectedBadge?.Image),
                    source: this)
            }
        );

        return a;
    }

    public IslandDisplayView GetIslandView()
    {
        return new IslandDisplayView(this)
            {
                BindingContext = this,
                DisplayIsland = DisplayUser?.SelectedIsland,
                DisplayPet = DisplayUser?.SelectedPet,
                DisplayDecor = DisplayUser?.SelectedFurniture
            }
            .Bind(IslandDisplayView.IslandProperty,
                getter: static (FriendProfilePage page) => page.DisplayUser,
                convert: static (User? user) => user?.SelectedIsland)
            .Bind(IslandDisplayView.PetProperty,
                getter: static (FriendProfilePage page) => page.DisplayUser,
                convert: static (User? user) => user?.SelectedPet)
            .Bind(IslandDisplayView.DisplayDecorProperty,
                getter: static (FriendProfilePage page) => page.DisplayUser,
                convert: static (User? user) => user?.SelectedFurniture);
    }

    private async void OnIdSet()
    {
        IsBusy = true;

        try
        {
            await GetUser();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error getting friend");
            // Go back a page
            await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
        }

        IsBusy = false;

        BuildUI();
    }

    public async Task GetUser(CancellationToken cancellationToken = default)
    {
        var user = await _mediator.Send(new Methods.User.GetAndSyncUser.Query()
        {
            Auth0Id = DisplayUserAuth0Id
        },
        cancellationToken);

        if (user != null)
        {
            DisplayUser = user;
        }
        else
        {
            await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
            throw new Exception( "Error getting user");
        }
    } 
}