using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Markup.LeftToRight;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using FocusApp.Client.Resources.FontAwesomeIcons;
using System.Runtime.CompilerServices;
using SimpleToolkit.SimpleShell.Extensions;
using Microsoft.Maui.Layouts;
using CommunityToolkit.Maui.Converters;
using FocusApp.Client.Views.Shop;
using FocusCore.Models;
using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;

namespace FocusApp.Client.Views.Social;

internal sealed class PetsPage : BasePage
{
    private readonly IAPIClient _client;
    private readonly IAuthenticationService _authenticationService;
    private readonly PopupService _popupService;
    private readonly FocusAppContext _localContext;

    FlexLayout _petsContainer;

    public PetsPage(IAPIClient client, IAuthenticationService authenticationService, PopupService popupService, FocusAppContext localContext)
	{
        _client = client;
        _authenticationService = authenticationService;
        _popupService = popupService;
        _localContext = localContext;

        // Instantiate container for pets
        _petsContainer = new FlexLayout
        {
            Direction = FlexDirection.Row,
            Wrap = FlexWrap.Wrap,
            JustifyContent = FlexJustify.SpaceAround
        };

        // Using grids
        Content = new Grid
        {
            // Define the length of the rows & columns
            // Rows: 80 for the top, 20 to act as padding, Stars for even spacing, and 140 for bottom padding
            // Columns: Two even columns should be enough
            RowDefinitions = Rows.Define(80, Star),
            ColumnDefinitions = Columns.Define(Star, Star),
            Children = {

				// Header
				new Label
                {
                    Text = "Pets",
                    TextColor = Colors.Black,
                    FontSize = 40
                }
                .Row(0)
                .Column(0)
                .ColumnSpan(3)
                .CenterVertical()
                .Paddings(top : 5, bottom: 5, left: 75, right: 5),


				// Back Button
				new Button
                {
                    Text = SolidIcons.ChevronLeft,
                    TextColor = Colors.Black,
                    FontFamily = nameof(SolidIcons),
                    FontSize = 40,
                    BackgroundColor = Colors.Transparent
                }
                .Left()
                .CenterVertical()
                .Column(0)
                .Paddings(top : 5, bottom: 5, left: 10, right: 5)
				// When clicked, go to social view
				.Invoke(button => button.Released += (sender, eventArgs) =>
                    BackButtonClicked(sender, eventArgs)),

				// Header & Content Divider
				new BoxView
                {
                    Color = Colors.Black,
                    WidthRequest = 400,
                    HeightRequest = 2
                }
                .Bottom()
                .Row(0)
                .Column(0)
                .ColumnSpan(int.MaxValue),
                //////////////////////////////////////////////////////////

				// FlexLayout - Container for Pets
                new ScrollView
                {
                    Content = _petsContainer
                }
                .Row(1)
                .Column(0)
                .ColumnSpan(2)
                .Center()
            }
		};
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Check if the user is logged in
        if (_authenticationService.CurrentUser == null)
        {
            var loginPopup = (EnsureLoginPopupInterface)_popupService.ShowAndGetPopup<EnsureLoginPopupInterface>();
            loginPopup.OriginPage = nameof(ShopPage);
            return;
        }

        // Fetch pets from the local database and display them
        var userPets = await FetchPetsFromLocalDb();
        DisplayPets(userPets);
    }

    private async Task<List<PetItem>> FetchPetsFromLocalDb()
    {
        List<PetItem> userPets = new List<PetItem>();

        userPets = SeedPetItems();

        try
        {
            // Fetch pets from the local database using Mediatr feature
            //userPets = await _localContext.Pets.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error when fetching pets from local DB.", ex);
        }

        return userPets;
    }

    private List<PetItem> SeedPetItems()
    {
        byte[]? image = _localContext.Pets.Where(p => p.Name == "Kyle").FirstOrDefault().Image;
        List<PetItem> petItems = new List<PetItem>();

        for (int i = 0; i < 5; i++)
        {
            petItems.Add(new PetItem
            {
                PetId = Guid.Empty,
                PetName = "TestPet " + i.ToString(),
                PetsProfilePicture = image
            });
        }

        return petItems;
    }

    private void DisplayPets(List<PetItem> userPets)
    {
        var petsContainer = _petsContainer;

        // Add pets to FlexLayout
        foreach (var pet in userPets)
        {
            var userPet = new ImageButton
            {
                Source = ImageSource.FromStream(() => new MemoryStream(pet.PetsProfilePicture)),
                Aspect = Aspect.AspectFit,
                WidthRequest = 150,
                HeightRequest = 150
            }
            .Invoke(button => button.Released += (s, e) => OnImageButtonClicked(s, e));
            petsContainer.Children.Add(userPet);
        }
    }

    void OnImageButtonClicked(object sender, EventArgs eventArgs)
    {
        var itemButton = sender as ImageButton;
        var pet = (PetItem)itemButton.BindingContext;

        /*var itemPopup = (UserBadgesPagePopupInterface)_popupService.ShowAndGetPopup<UserBadgesPagePopupInterface>();
        itemPopup.UserBadgesPage = this;
        itemPopup.PopulatePopup(badge);*/
    }

    private async void BackButtonClicked(object sender, EventArgs e)
    {
        // Back navigation reverses animation so can keep right to left
        Shell.Current.SetTransition(Transitions.LeftToRightPlatformTransition);
        await Shell.Current.GoToAsync($"///{nameof(SocialPage)}/{nameof(SocialPage)}");
    }
}
