using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using CommunityToolkit.Maui.Views;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Client.Views.Controls;
using FocusApp.Shared.Models;
using FocusCore.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using SimpleToolkit.SimpleShell;
using SimpleToolkit.SimpleShell.Extensions;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

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

    enum Column { ProfilePicture, Pronouns }
    enum Row { TopBar, ProfilePicAndPronouns, IslandView, ProfileInfo, BottomWhiteSpace }

    // Row / Column structure for user data header
    enum UserDataRow { UserEmail, UserName, UserPronouns }
    enum UserDataColumn { Data, UtilityButtons }


    int _labelFontSize = 17;
    int _valueFontSize = 20;
    Color _labelColor = Colors.SlateGray;

    private readonly IMediator _mediator;
    private readonly ILogger<FriendProfilePage> _logger;

    public Grid GetBaseGrid()
    {
        return new Grid()
        {
            RowDefinitions = GridRowsColumns.Rows.Define(
                (Row.TopBar, 80),
                (Row.ProfilePicAndPronouns, GridRowsColumns.Stars(1)),
                (Row.IslandView, GridRowsColumns.Stars(2)),
                (Row.ProfileInfo, GridRowsColumns.Stars(1)),
                (Row.BottomWhiteSpace, 80)
            ),
            ColumnDefinitions = GridRowsColumns.Columns.Define(
                (Column.ProfilePicture, GridRowsColumns.Stars(2)),
                (Column.Pronouns, GridRowsColumns.Stars(2))
            ),
            Children =
            {
                new ActivityIndicator()
                    {
                        Color = AppStyles.Palette.OrchidPink
                    }
                    .ZIndex(2)
                    .Row(Row.ProfilePicAndPronouns, Row.BottomWhiteSpace)
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
            // Header & Content Divider
            new BoxView
                {
                    Color = Color.Parse("Black"),
                    HeightRequest = 2,
                }
                .FillHorizontal()
                .Bottom()
                .Row(Row.TopBar)
                .ColumnSpan(typeof(Column).GetEnumNames().Length));

        Border profilePic = GetProfilePic()
            .Row(Row.ProfilePicAndPronouns)
            .Column(Column.ProfilePicture);

        grid.Children.Add(profilePic);

        grid.Children.Add(
            GetBadgeOverlay(profilePic)
                .Row(Row.ProfilePicAndPronouns)
                .Column(Column.ProfilePicture));

        grid.Children.Add(
            GetPronounsContainer()
                .Row(Row.ProfilePicAndPronouns)
                .Column(Column.Pronouns));

        grid.Children.Add(
            GetIslandView()
                .CenterVertical()
                .Row(Row.IslandView)
                .ColumnSpan(typeof(Column).GetEnumNames().Length));

        grid.Children.Add(
            GetProfileInfo()
                .Row(Row.ProfileInfo)
                .ColumnSpan(typeof(Column).GetEnumNames().Length));


        MainThread.BeginInvokeOnMainThread(() => Content = grid);
    }

    public View GetTopBar()
    {
        return new FlexLayout
        {
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
                        OnBackButtonClicked()),

                // Header
                new Label
                    {
                        TextColor = Colors.Black,
                        FontSize = 20
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

    public AvatarView GetProfilePic()
    {
        return new AvatarView
            {
                CornerRadius = 63,
                HeightRequest = 126,
                WidthRequest = 126,
                ImageSource = new ByteArrayToImageSourceConverter().ConvertFrom(DisplayUser?.ProfilePicture),
            }
            .Aspect(Aspect.AspectFit)
            .Top()
            .Margins(top: 10, left: -5);
    }

    public FlexLayout GetBadgeOverlay(View profilePic)
    {
        return new FlexLayout()
            {
                ZIndex = 1,
                HeightRequest = profilePic.HeightRequest,
                WidthRequest = profilePic.WidthRequest,
                Direction = FlexDirection.Column,
                JustifyContent = FlexJustify.End,
                Children =
                {
                    new Frame()
                    {
                        WidthRequest = HeightRequest,
                        Padding = 0,
                        BackgroundColor = Colors.Transparent,
                        Content = new Image()
                        {
                            Aspect = Aspect.AspectFit,
                            Source = new ByteArrayToImageSourceConverter().ConvertFrom(DisplayUser?.SelectedBadge?.Image)
                        }
                    }
                    .Margins(right: -10, bottom: -10)
                    .Basis(0.35f, true)
                    .Right()
                }

            }
            .Top()
            .Margins(top: 10, left: -5)
            .Bind(
                FlexLayout.HeightRequestProperty,
                getter: static (profilePic) => profilePic.HeightRequest,
                source: profilePic);
    }

    public FlexLayout GetPronounsContainer()
    {
        int horizontalPadding = 5;
        int verticalPadding = 2;

        return new FlexLayout()
            {
                Direction = FlexDirection.Column,
                AlignItems = FlexAlignItems.Start,
                JustifyContent = FlexJustify.Start,
                Children =
                {
                    // Pronouns
                    new Label()
                        {
                            Text = "Pronouns",
                            FontSize = _labelFontSize,
                            TextColor = _labelColor
                        }
                        .Padding(horizontalPadding, verticalPadding)
                        .AlignSelf(FlexAlignSelf.Start),
                    new Label()
                        {
                            Text = $"{DisplayUser?.Pronouns}",
                            FontSize = _valueFontSize
                        }
                        .Padding(horizontalPadding, verticalPadding)
                        .AlignSelf(FlexAlignSelf.Start),
                }
            }
            .Margins(top: 10);
    }

    public FlexLayout GetProfileInfo()
    {
        int horizontalPadding = 5;
        int verticalPadding = 2;

        var a = new FlexLayout()
        {
            Direction = FlexDirection.Column,
            AlignItems = FlexAlignItems.End,
            JustifyContent = FlexJustify.Start,
            Children =
            {
                // Email / Id
                new Label()
                    {
                        Text = "Friend Id",
                        FontSize = _labelFontSize,
                        TextColor = _labelColor
                    }
                    .Padding(horizontalPadding, verticalPadding)
                    .AlignSelf(FlexAlignSelf.Start),
                new Label()
                    {
                        Text = $"#{DisplayUser?.Email}",
                        FontSize = _valueFontSize
                    }
                    .Padding(horizontalPadding, verticalPadding)
                    .AlignSelf(FlexAlignSelf.Start),

                // Date Joined
                new Label
                    {
                        Text = $"Member Since: {DisplayUser?.DateCreated.ToShortDateString()}",
                        TextColor = Colors.Black,
                        FontSize = 20
                    }
                    .Margins(top: 10)
                    .AlignSelf(FlexAlignSelf.Center)


            }
        }
        .Margins(top: 10, left: 10);

        return a;
    }

    public IslandDisplayView GetIslandView()
    {
        return new IslandDisplayView(this)
            {
                BindingContext = this,
                DisplayIsland = DisplayUser?.SelectedIsland,
                DisplayPet = DisplayUser?.SelectedPet,
                DisplayDecor = DisplayUser?.SelectedDecor
            }
            .Bind(IslandDisplayView.IslandProperty,
                getter: static (FriendProfilePage page) => page.DisplayUser,
                convert: static (User? user) => user?.SelectedIsland)
            .Bind(IslandDisplayView.PetProperty,
                getter: static (FriendProfilePage page) => page.DisplayUser,
                convert: static (User? user) => user?.SelectedPet)
            .Bind(IslandDisplayView.DisplayDecorProperty,
                getter: static (FriendProfilePage page) => page.DisplayUser,
                convert: static (User? user) => user?.SelectedDecor);
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

            await DisplayAlert("Error", ex.Message, "OK");

            // Go back a page
            await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
        }

        IsBusy = false;

        BuildUI();
    }

    public async Task GetUser(CancellationToken cancellationToken = default)
    {
        var user = await _mediator.Send(new Methods.User.GetFriend.Query()
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

    private async Task OnBackButtonClicked()
    {
        Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
    }

    protected override async void OnAppearing()
    {
        await AppShell.Current.SetTabBarIsVisible(false);
    }
}