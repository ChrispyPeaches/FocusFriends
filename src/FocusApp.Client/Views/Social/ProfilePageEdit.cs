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
using CommunityToolkit.Maui.Behaviors;
using MediatR;
using Microsoft.Extensions.Logging;
using FocusCore.Commands.User;
using System.Text;
using FocusApp.Client.Views;
using FocusCore.Responses;
using FocusCore.Responses.User;
using FocusApp.Methods.User;

namespace FocusApp.Client.Views.Social;

internal class ProfilePageEdit : BasePage
{
    IAuthenticationService _authenticationService;
    IMediator _mediator;
    ILogger<ProfilePageEdit> _logger;
    PopupService _popupService;

    // Page Row / Column Definitions
    enum PageRow { PageHeader, ProfilePicture, FormFields, Logo, TabBarSpace }
    enum PageColumn { Left, Right }

    // Form Fields Row / Column Definitions
    enum FormRow { Username, Pronouns, SaveButton }
    enum FormColumn { Label, Remainder }

    TextValidationBehavior _userNameValidationBehavior { get; set; }
    TextValidationBehavior _pronounsValidationBehavior { get; set; }
    Entry _userNameField { get; set; }
    Entry _pronounsField { get; set; }
    AvatarView _profilePicture { get; set; }
    byte[]? _newPhoto { get; set; } = null;

    #region Frontend
    public ProfilePageEdit(IAuthenticationService authenticationService, IMediator mediator, ILogger<ProfilePageEdit> logger, PopupService popupService)
    {
        _authenticationService = authenticationService;
        _mediator = mediator;
        _logger = logger;
        _popupService = popupService;

        CreateFormValidationBehaviors();
        CreateFormElements();

        // Bind profile picture to avatar view
        _profilePicture = new AvatarView
        {
            CornerRadius = 63,
            HeightRequest = 126,
            WidthRequest = 126
        }
        .Bind(AvatarView.ImageSourceProperty, "ProfilePicture", converter: new ByteArrayToImageSourceConverter());
        _profilePicture.BindingContext = _authenticationService.CurrentUser;

        Content = new Grid
        {
            // Define the length of the rows & columns
            RowDefinitions = Rows.Define(
                (PageRow.PageHeader, Stars(0.2)),
                (PageRow.ProfilePicture, Stars(0.5)),
                (PageRow.FormFields, Stars(1)),
                (PageRow.Logo, Stars(0.75)),
                (PageRow.TabBarSpace, Stars(0.25))
                ),
            ColumnDefinitions = Columns.Define(
                (PageColumn.Left, Stars(1)),
                (PageColumn.Right, Stars(1))
                ),
            BackgroundColor = AppStyles.Palette.Celeste,

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
                _profilePicture
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
                .Center()
                .Invoke(button => button.Released += (sender, eventArgs) =>
                    ChangeProfilePictureClicked(sender, eventArgs)),

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
                                    BackgroundColor = AppStyles.Palette.LightPeriwinkle,
                                    Stroke = Colors.Transparent,
                                    StrokeThickness = 2,
                                    Content = _userNameField
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
                                    BackgroundColor = AppStyles.Palette.LightPeriwinkle,
                                    Stroke = Colors.Transparent,
                                    StrokeThickness = 2,
                                    Content = _pronounsField
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
                .Row(PageRow.Logo)
                .ColumnSpan(typeof(PageColumn).GetEnumNames().Length)
                .Center()
            }
        };
    }

    private void CreateFormValidationBehaviors()
    {
        _userNameValidationBehavior = new TextValidationBehavior
        {
            MaximumLength = 20,
            MinimumLength = 3,
            Flags = ValidationFlags.ValidateOnValueChanged,
            InvalidStyle = new Style<Entry>(Entry.TextColorProperty, Colors.Red),
            ValidStyle = new Style<Entry>(Entry.TextColorProperty, Colors.Black)
        };

        _pronounsValidationBehavior = new TextValidationBehavior
        {
            MaximumLength = 10,
            Flags = ValidationFlags.ValidateOnValueChanged,
            InvalidStyle = new Style<Entry>(Entry.TextColorProperty, Colors.Red),
            ValidStyle = new Style<Entry>(Entry.TextColorProperty, Colors.Black)
        };
    }

    private void CreateFormElements()
    {
        _userNameField = new Entry
        {
            Placeholder = "Enter username here",
            Text = _authenticationService.CurrentUser?.UserName
        };

        _pronounsField = new Entry
        {
            Placeholder = "Enter pronouns here",
            Text = _authenticationService.CurrentUser?.Pronouns
        };

        _userNameField.Behaviors.Add(_userNameValidationBehavior);
        _pronounsField.Behaviors.Add(_pronounsValidationBehavior);
    }

    protected override async void OnAppearing()
    {
        await AppShell.Current.SetTabBarIsVisible(false);

        // Refresh profile picture on page load in case it was wiped via navigating to settings
        _profilePicture.ImageSource = new ByteArrayToImageSourceConverter().ConvertFrom(_authenticationService.CurrentUser?.ProfilePicture);
    }

    #endregion

    #region Backend

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        // Back navigation reverses animation so can keep right to left
        Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(ProfilePage)}");
    }

    private async void ChangeProfilePictureClicked(object sender, EventArgs e)
    {
        FileResult result = await MediaPicker.Default.PickPhotoAsync();

        if (result != null)
        {
            try
            {
                // Open the selected photo as a stream
                Stream photoStream = await result.OpenReadAsync();
                // Initialize a byte array for the stream
                _newPhoto = new byte[photoStream.Length];
                // Read the photostream into the byte array
                photoStream.Read(buffer: _newPhoto, offset: 0, count: _newPhoto.Length);

                // Change image to uploaded image on screen, before form-submit
                _profilePicture.ImageSource = new ByteArrayToImageSourceConverter().ConvertFrom(_newPhoto);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while selecting an image from the photo gallery. Message: " + ex.Message);
                await DisplayAlert("Error", "An error occurred while uploading the selected image.", "Ok");
            }
        }
    }

    private async void SaveChangesButtonClicked(object sender, EventArgs e)
    {
        if (_userNameValidationBehavior.IsNotValid || _pronounsValidationBehavior.IsNotValid)
        {
            StringBuilder errorMessage = new StringBuilder();

            if (_userNameValidationBehavior.IsNotValid)
                errorMessage.AppendLine("Username must be less than 25 characters.");

            if (_pronounsValidationBehavior.IsNotValid)
                errorMessage.AppendLine("Pronouns must be less than 10 characters.");

            await DisplayAlert("Error", errorMessage.ToString(), "Ok");
            return;
        }
        else
        {
            _popupService.ShowPopup<GenericLoadingPopupInterface>();
            var result = await UpdateUser();
            _popupService.HidePopup();
            if (result.IsSuccessful)
            {
                await DisplayAlert("Information", result.Message, "Ok");
            }
            else
            {
                await DisplayAlert("Error", result.Message, "Ok");
            }
        }
    }

    private async Task<EditUserProfile.Response> UpdateUser()
    {
        EditUserProfileCommand command = new EditUserProfileCommand
        {
            UserId = _authenticationService.CurrentUser?.Id,
        };

        if (_userNameField.Text != _authenticationService.CurrentUser?.UserName)
            command.UserName = _userNameField.Text;

        if (_pronounsField.Text != _authenticationService.CurrentUser?.Pronouns)
            command.Pronouns = _pronounsField.Text;

        if (_newPhoto != _authenticationService.CurrentUser?.ProfilePicture)
            command.ProfilePicture = _newPhoto;

        if (!string.IsNullOrEmpty(command.Pronouns) || !string.IsNullOrEmpty(command.UserName) || command.ProfilePicture != null)
        {
            // Call method to update the user data when the user fields have changed
            MediatrResult result = await _mediator.Send(command, default);
            return new EditUserProfile.Response
            { 
                Message = result.Success ? "Successfully saved changes!" : "There was an issue saving your changes.",
                IsSuccessful = result.Success
            };
        }
        else
        {
            return new EditUserProfile.Response
            {
                Message = "No changes were made to your user information.",
                IsSuccessful = true
            };
        }
    }
    #endregion
}
