using FocusAPI.Helpers;
using FocusAPI.Models;
using FocusAPI.Tests.Fakers.BaseModels;
using FocusAPI.Tests.Fakers.ImplementedModels;
using FocusCore.Models;
using Shouldly;

namespace FocusAPI.Tests.ProjectionHelperMethods;
public class ProjectionHelperTests
{
    UserFaker _userFaker;
    BaseUserFaker _baseUserFaker;
    public ProjectionHelperTests()
    {
        SetupTestHelpers();
    }

    void SetupTestHelpers()
    { 
        _userFaker = new UserFaker();
        _baseUserFaker = new BaseUserFaker();
    }

    [Fact]
    public void ProjectToBaseUser_CorrectlyProjectsAllFields()
    {
        // ARRANGE
        User user = _userFaker.Generate();

        // ACT
        BaseUser projectedUser = ProjectionHelper.ProjectToBaseUser(user);

        // ASSERT
        projectedUser.Id.ShouldBe(user.Id);
        projectedUser.Auth0Id.ShouldBe(user.Auth0Id);
        projectedUser.UserName.ShouldBe(user.UserName);
        projectedUser.Email.ShouldBe(user.Email);
        projectedUser.DateCreated.ShouldBe(user.DateCreated);
        projectedUser.Balance.ShouldBe(user.Balance);
        projectedUser.Pronouns.ShouldBe(user.Pronouns);
        projectedUser.ProfilePicture.ShouldBe(user.ProfilePicture);
        projectedUser.SelectedPetId.ShouldBe(user.SelectedPetId);
        projectedUser.SelectedIslandId.ShouldBe(user.SelectedIslandId);
        projectedUser.SelectedDecorId.ShouldBe(user.SelectedDecorId);
        projectedUser.SelectedBadgeId.ShouldBe(user.SelectedBadgeId);

        projectedUser.Pets.ShouldNotBeNull();
        projectedUser.Pets.Count.ShouldBe(user.Pets.Count);
        for (int i = 0; i < projectedUser.Pets.Count; i++)
        {
            UserPet userPet = user.Pets.ElementAt(i);
            BaseUserPet projectedUserPet = projectedUser.Pets.ElementAt(i);
            projectedUserPet.UserId.ShouldBe(userPet.UserId);
            projectedUserPet.PetId.ShouldBe(userPet.PetId);
            /* Check these assertions w/ team - do we want to do a complete mapping of all items when projecting?
            projectedUserPet.Pet.Id.ShouldBe(userPet.Pet.Id);
            projectedUserPet.Pet.Price.ShouldBe(userPet.Pet.Price);
            projectedUserPet.Pet.Name.ShouldBe(userPet.Pet.Name);
            projectedUserPet.Pet.Image.ShouldBe(userPet.Pet.Image);
            projectedUserPet.Pet.HeightRequest.ShouldBe(userPet.Pet.HeightRequest);
            */
        }

        projectedUser.Islands.ShouldNotBeNull();
        projectedUser.Islands.Count.ShouldBe(user.Islands.Count);
        for (int i = 0; i < projectedUser.Islands.Count; i++)
        {
            UserIsland userIsland = user.Islands.ElementAt(i);
            BaseUserIsland projectedUserIsland = projectedUser.Islands.ElementAt(i);
            projectedUserIsland.UserId.ShouldBe(userIsland.UserId);
            projectedUserIsland.IslandId.ShouldBe(userIsland.IslandId);
            /*
            projectedUserIsland.Island.Id.ShouldBe(userIsland.Island.Id);
            projectedUserIsland.Island.Price.ShouldBe(userIsland.Island.Price);
            projectedUserIsland.Island.Name.ShouldBe(userIsland.Island.Name);
            projectedUserIsland.Island.Image.ShouldBe(userIsland.Island.Image);
            */
        }

        projectedUser.Decor.ShouldNotBeNull();
        projectedUser.Decor.Count.ShouldBe(user.Decor.Count);
        for (int i = 0; i < projectedUser.Decor.Count; i++)
        {
            UserDecor userDecor = user.Decor.ElementAt(i);
            BaseUserDecor projectedUserDecor = projectedUser.Decor.ElementAt(i);
            projectedUserDecor.UserId.ShouldBe(userDecor.UserId);
            projectedUserDecor.DecorId.ShouldBe(userDecor.DecorId);
            /*
            projectedUserDecor.Decor.Id.ShouldBe(userDecor.Decor.Id);
            projectedUserDecor.Decor.Price.ShouldBe(userDecor.Decor.Price);
            projectedUserDecor.Decor.Name.ShouldBe(userDecor.Decor.Name);
            projectedUserDecor.Decor.Image.ShouldBe(userDecor.Decor.Image);
            projectedUserDecor.Decor.HeightRequest.ShouldBe(userDecor.Decor.HeightRequest);
            */
        }

        projectedUser.Badges.ShouldNotBeNull();
        projectedUser.Badges.Count.ShouldBe(user.Badges.Count);
        for (int i = 0; i < projectedUser.Badges.Count; i++)
        {
            UserBadge userBadge = user.Badges.ElementAt(i);
            BaseUserBadge projectedUserBadge = projectedUser.Badges.ElementAt(i);
            projectedUserBadge.UserId.ShouldBe(userBadge.UserId);
            projectedUserBadge.BadgeId.ShouldBe(userBadge.BadgeId);
            /*
            projectedUserBadge.Badge.Id.ShouldBe(userBadge.Badge.Id);
            projectedUserBadge.Badge.Name.ShouldBe(userBadge.Badge.Name);
            projectedUserBadge.Badge.Description.ShouldBe(userBadge.Badge.Description);
            */
        }
    }
}