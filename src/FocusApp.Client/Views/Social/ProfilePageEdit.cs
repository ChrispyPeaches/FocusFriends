using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Client.Resources;
using FocusApp.Client.Resources.FontAwesomeIcons;
using FocusApp.Shared.Data;
using SimpleToolkit.SimpleShell.Extensions;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Shapes;
using CommunityToolkit.Maui.Behaviors;
using FocusApp.Client.Methods.User;
using MediatR;
using Microsoft.Extensions.Logging;
using FocusCore.Commands.User;

namespace FocusApp.Client.Views.Social;

internal class ProfilePageEdit : BasePage
{

    IAPIClient _client;
    IAuthenticationService _authenticationService;
    IMediator _mediator;
    ILogger<ProfilePageEdit> _logger;

    // Page Row / Column Definitions
    enum PageRow { PageHeader, ProfilePicture, FormFields, TabBarSpace }
    enum PageColumn { Left, Right }

    // Form Fields Row / Column Definitions
    enum FormRow { Username, Pronouns, SaveButton }
    enum FormColumn { Label, Remainder }

    Entry userNameField { get; set; }
    Entry pronounsField { get; set; }

    #region Frontend
    public ProfilePageEdit(IAPIClient client, IAuthenticationService authenticationService, IMediator mediator, ILogger<ProfilePageEdit> logger)
    {
        _client = client;
        _authenticationService = authenticationService;
        _mediator = mediator;
        _logger = logger;

        // Set bindable properties with images
        AvatarView profilePicture = new AvatarView
        {
            CornerRadius = 63,
            HeightRequest = 126,
            WidthRequest = 126
        }
        .Bind(AvatarView.ImageSourceProperty, "ProfilePicture", converter: new ByteArrayToImageSourceConverter());
        profilePicture.BindingContext = _authenticationService.CurrentUser;

        TextValidationBehavior userNameValidationBehavior = new TextValidationBehavior
        {
            MaximumLength = 50,
            MinimumLength = 3
        };

        TextValidationBehavior pronounsValidationBehavior = new TextValidationBehavior
        {
            MaximumLength = 16,
        };

        userNameField = new Entry
        {
            Placeholder = "Enter username here",
            Text = _authenticationService.CurrentUser?.UserName
        };

        userNameField.Behaviors.Add(userNameValidationBehavior);

        pronounsField = new Entry
        {
            Placeholder = "Enter pronouns here",
            Text = _authenticationService.CurrentUser?.Pronouns
        };

        pronounsField.Behaviors.Add(pronounsValidationBehavior);

        Content = new Grid
        {
            // Define the length of the rows & columns
            RowDefinitions = Rows.Define(
                (PageRow.PageHeader, Stars(0.2)),
                (PageRow.ProfilePicture, Stars(0.5)),
                (PageRow.FormFields, Stars(1)),
                (PageRow.TabBarSpace, Stars(1))
                ),
            ColumnDefinitions = Columns.Define(
                (PageColumn.Left, Stars(1)),
                (PageColumn.Right, Stars(1))
                ),
            BackgroundColor = AppStyles.Palette.LightMauve,

            Children =
            {
                // Back Button
                new Button
                {
                     Text = SolidIcons.ChevronLeft,
                     TextColor = Colors.Black,
                     FontFamily = nameof(SolidIcons),
                     FontSize = 30,
                     BackgroundColor = Colors.Transparent
                }
                .Row(PageRow.PageHeader)
                .Column(PageColumn.Left)
                .CenterVertical()
                .Left()
                // When clicked, go to timer view
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    BackButtonClicked(sender, eventArgs)),

                new Label
                { 
                    Text = "Edit Profile",
                    FontSize = 20
                }
                .Row(PageRow.PageHeader)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                .Center(),

                new BoxView
                { 
                    Color = Colors.Black,
                    HeightRequest = 2
                }
                .Row(PageRow.PageHeader)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                .Bottom(),

                // Profile Picture
                profilePicture
                .Row(PageRow.ProfilePicture)
                .Column(PageColumn.Right)
                .Center(),

                new Button
                { 
                    Text = SolidIcons.Plus,
                    TextColor = Colors.Black,
                    FontSize = 30,
                    Opacity = 0.35,
                    CornerRadius = 63,
                    WidthRequest = 126,
                    HeightRequest = 126,
                    BackgroundColor = Colors.LightGray
                }
                .Row(PageRow.ProfilePicture)
                .Column(PageColumn.Right)
                .Center(),

                new Grid
                { 
                    RowDefinitions = Rows.Define(
                        (FormRow.Username, Stars(1)),
                        (FormRow.Pronouns, Stars(1)),
                        (FormRow.SaveButton, Stars(1))
                        ),
                    ColumnDefinitions = Columns.Define(
                        (PageColumn.Left, Stars(1)),
                        (PageColumn.Right, Stars(1))
                        ),
                    Children =
                    {
                        new StackLayout
                        { 
                            Children =
                            {
                                new Label
                                {
                                    Text = "Username",
                                    FontSize = 20,
                                },
                                new Border
                                {
                                    BackgroundColor = AppStyles.Palette.DarkMauve,
                                    Stroke = Colors.Transparent,
                                    StrokeThickness = 2,
                                    Content = userNameField
                                }
                            }
                        }
                        .Row(FormRow.Username)
                        .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                        .CenterVertical()
                        .Margins(left:10, right: 10),

                        new StackLayout
                        { 
                            Children =
                            {
                                new Label
                                {
                                    Text = "Pronouns",
                                    FontSize = 20,
                                },
                                new Border
                                {
                                    BackgroundColor = AppStyles.Palette.DarkMauve,
                                    Stroke = Colors.Transparent,
                                    StrokeThickness = 2,
                                    Content = pronounsField
                                }
                            }
                        }
                        .Row(FormRow.Pronouns)
                        .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                        .CenterVertical()
                        .Margins(left:10, right: 10),

                        new Button
                        { 
                            Text = "Save Changes"
                        }
                        .Row(FormRow.SaveButton)
                        .ColumnSpan(typeof(FormColumn).GetEnumNames().Length)
                        .Center()
                        .Invoke(button => button.Released += (sender, eventArgs) =>
                            SaveChangesButtonClicked(sender, eventArgs))
                    }
                }
                .Row(PageRow.FormFields)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                .Left()
                .FillHorizontal(),

                new Image
                {
                    Source = new FileImageSource
                    {
                        File = "logo.png"
                    },
                    HeightRequest = 75,
                    WidthRequest = 75,
                }
                .Row(PageRow.TabBarSpace)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                .Top()
                .CenterHorizontal()
                .Margins(top: 15)
            }
        };
    }

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        // Back navigation reverses animation so can keep right to left
        Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(ProfilePage)}");
    }

    private async void SaveChangesButtonClicked(object sender, EventArgs e)
    {
        EditUserProfileCommand command = new EditUserProfileCommand
        { 
            UserId = _authenticationService.CurrentUser?.Id
        };

        if (userNameField.Text != _authenticationService.CurrentUser?.UserName)
            command.UserName = userNameField.Text;

        if (pronounsField.Text != _authenticationService.CurrentUser?.Pronouns)
            command.Pronouns = pronounsField.Text;

        if (!string.IsNullOrEmpty(command.Pronouns) || !string.IsNullOrEmpty(command.UserName))
        {
            try
            {
                await _client.EditUserProfile(command, default);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Debug, "Error occurred when editing user profile. Message: " + ex.Message);
            }
        }
    }

    #endregion

    #region Backend
    #endregion
}
