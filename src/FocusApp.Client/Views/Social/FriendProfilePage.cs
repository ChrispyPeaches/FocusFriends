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
using Microsoft.Maui.Layouts;
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

    enum Column { ProfilePicture, ProfileInfo }
    enum Row { TopBar, ProfileInfo, IslandView, BottomWhiteSpace }

    // Row / Column structure for user data header
    enum UserDataRow { UserEmail, UserName, UserPronouns }
    enum UserDataColumn { Data, UtilityButtons }

    private readonly IMediator _mediator;
    private readonly ILogger<FriendProfilePage> _logger;

    public Grid GetBaseGrid()
    {
        return new Grid()
        {
            RowDefinitions = GridRowsColumns.Rows.Define(
                (Row.TopBar, 80),
                (Row.ProfileInfo, GridRowsColumns.Stars(3)),
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

        AvatarView profilePic = GetProfilePic()
            .Row(Row.ProfileInfo)
            .Column(Column.ProfilePicture);
        grid.Children.Add(profilePic);
        grid.Children.Add(
            GetBadgeOverlay(profilePic)
                .Row(Row.ProfileInfo)
                .Column(Column.ProfilePicture));
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
            .Top()
            .Margins(top: 10, left: -5);
    }

    public FlexLayout GetBadgeOverlay(AvatarView profilePic)
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

    public FlexLayout GetProfileInfo()
    {
        int horizontalPadding = 5;
        int verticalPadding = 2;
        int labelFontSize = 15;
        int valueFontSize = 17;
        Color labelColor = Colors.SlateGray;

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
                        Text = $"#{DisplayUser?.Email}",
                        FontSize = valueFontSize
                    }
                    .Padding(horizontalPadding, verticalPadding)
                    .AlignSelf(FlexAlignSelf.Start),

                // Name
                new Label()
                    {
                        Text = "Name",
                        FontSize = labelFontSize,
                        TextColor = labelColor
                    }
                    .Padding(horizontalPadding, verticalPadding)
                    .AlignSelf(FlexAlignSelf.Start),
                new Label()
                    {
                        Text = DisplayUser?.FullName(),
                        FontSize = valueFontSize
                    }
                    .Padding(horizontalPadding, verticalPadding)
                    .AlignSelf(FlexAlignSelf.Start),
                
                // Pronouns
                new Label()
                    {
                        Text = "Pronouns",
                        FontSize = labelFontSize,
                        TextColor = labelColor
                    }
                    .Padding(horizontalPadding, verticalPadding)
                    .AlignSelf(FlexAlignSelf.Start),
                new Label()
                    {
                        Text = $"{DisplayUser?.Pronouns}",
                        FontSize = valueFontSize
                    }
                    .Padding(horizontalPadding, verticalPadding)
                    .AlignSelf(FlexAlignSelf.Start),


            }
        }
        .Margins(top: 10);

        return a;
    }

    public Grid GetProfileInfo2()
    {
        return new Grid
        {
            RowDefinitions = Rows.Define(
                (UserDataRow.UserEmail, Stars(1)),
                (UserDataRow.UserName, Stars(1.5)),
                (UserDataRow.UserPronouns, Stars(1))
            ),
            ColumnDefinitions = Columns.Define(
                (UserDataColumn.Data, Stars(2)),
                (UserDataColumn.UtilityButtons, Stars(0.75))
            ),
            Children =
            {
                new Label
                    {
                        Text = $"#{DisplayUser?.Email}",
                        FontSize = 15
                    }
                    .Row(UserDataRow.UserEmail)
                    .ColumnSpan(typeof(UserDataColumn).GetEnumNames().Length - 1)
                    .CenterVertical()
                    .Left(),
                new Label
                    {
                        Text = $"{DisplayUser?.UserName}",
                        FontSize = 20
                    }
                    .Row(UserDataRow.UserName)
                    .ColumnSpan(typeof(UserDataColumn).GetEnumNames().Length)
                    .CenterVertical()
                    .Left(),
                new Label
                    {
                        Text = $"Pronouns: {DisplayUser?.Pronouns}",
                        FontSize = 15
                    }
                    .Row(UserDataRow.UserPronouns)
                    .ColumnSpan(typeof(UserDataColumn).GetEnumNames().Length - 1)
                    .Top()
                    .Left()
            }
        };
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
        var user = await _mediator.Send(new Methods.User.GetAndSyncFriend.Query()
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
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
    }
}