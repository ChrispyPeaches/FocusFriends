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
    FriendshipFaker _friendshipFaker;
    BaseUserFaker _baseUserFaker;
    BaseFriendshipFaker _baseFriendshipFaker;

    public ProjectionHelperTests()
    {
        SetupTestHelpers();
    }

    void SetupTestHelpers()
    {
        _userFaker = new();
        _baseUserFaker = new();
    }

    [Fact]
    public void ProjectToBaseUser_CorrectlyProjectsAllFields()
    {
        // ARRANGE

        // Create fake user model
        User source = _userFaker.Generate();

        // Create fake user friendships
        _friendshipFaker = new(source);
        source.Invitees = _friendshipFaker.Generate(1);
        source.Inviters = _friendshipFaker.Generate(1);

        // ACT
        BaseUser destination = ProjectionHelper.ProjectToBaseUser(source);

        // ASSERT
        VerifyUserAtomicFields_ProjectedCorrectly(source, destination);
        VerifyUserPets_ProjectedCorrectly(source, destination);
        VerifyUserIslands_ProjectedCorrectly(source, destination);
        VerifyUserDecor_ProjectedCorrectly(source, destination);
        VerifyUserBadges_ProjectedCorrectly(source, destination);
        VerifyUserSelectedItems_ProjectedCorrectly(source, destination);
        VerifyUserFriendships_ProjectedCorrectly(source, destination);
        VerifyUserSessions_ProjectedCorrectly(source, destination);
    }

    [Fact]
    public void ProjectFromBaseUser_CorrectlyProjectsAllFields()
    {
        // ARRANGE

        // Create fake base user model
        BaseUser source = _baseUserFaker.Generate();

        // Create fake user friendships
        _baseFriendshipFaker = new(source);
        source.Invitees = _baseFriendshipFaker.Generate(1);
        source.Inviters = _baseFriendshipFaker.Generate(1);

        // ACT
        User destination = ProjectionHelper.ProjectFromBaseUser(source);

        // ASSERT
        VerifyBaseUserAtomicFields_ProjectedCorrectly(source, destination);
        VerifyBaseUserPets_ProjectedCorrectly(source, destination);
        VerifyBaseUserIslands_ProjectedCorrectly(source, destination);
        VerifyBaseUserDecor_ProjectedCorrectly(source, destination);
        VerifyBaseUserBadges_ProjectedCorrectly(source, destination);
        VerifyBaseUserSelectedItems_ProjectedCorrectly(source, destination);
        VerifyBaseUserFriendships_ProjectedCorrectly(source, destination);
        VerifyBaseUserSessions_ProjectedCorrectly(source, destination);
    }

    #region User -> BaseUser Projection Assertions

    private void VerifyUserAtomicFields_ProjectedCorrectly(User source, BaseUser destination)
    {
        destination.Id.ShouldBe(source.Id);
        destination.Auth0Id.ShouldBe(source.Auth0Id);
        destination.UserName.ShouldBe(source.UserName);
        destination.Email.ShouldBe(source.Email);
        destination.DateCreated.ShouldBe(source.DateCreated);
        destination.Balance.ShouldBe(source.Balance);
        destination.Pronouns.ShouldBe(source.Pronouns);
        destination.ProfilePicture.ShouldBe(source.ProfilePicture);
        destination.SelectedPetId.ShouldBe(source.SelectedPetId);
        destination.SelectedIslandId.ShouldBe(source.SelectedIslandId);
        destination.SelectedDecorId.ShouldBe(source.SelectedDecorId);
        destination.SelectedBadgeId.ShouldBe(source.SelectedBadgeId);
    }

    private void VerifyUserPets_ProjectedCorrectly(User source, BaseUser destination)
    {
        destination.Pets.ShouldNotBeNull();
        destination.Pets.Count.ShouldBe(source.Pets.Count);
        for (int i = 0; i < destination.Pets.Count; i++)
        {
            UserPet sourcePet = source.Pets.ElementAt(i);
            BaseUserPet destinationPet = destination.Pets.ElementAt(i);
            destinationPet.UserId.ShouldBe(sourcePet.UserId);
            destinationPet.PetId.ShouldBe(sourcePet.PetId);
        }
    }

    private void VerifyUserIslands_ProjectedCorrectly(User source, BaseUser destination)
    {
        destination.Islands.ShouldNotBeNull();
        destination.Islands.Count.ShouldBe(source.Islands.Count);
        for (int i = 0; i < destination.Islands.Count; i++)
        {
            UserIsland sourceIsland = source.Islands.ElementAt(i);
            BaseUserIsland destinationIsland = destination.Islands.ElementAt(i);
            destinationIsland.UserId.ShouldBe(sourceIsland.UserId);
            destinationIsland.IslandId.ShouldBe(sourceIsland.IslandId);
        }
    }

    private void VerifyUserDecor_ProjectedCorrectly(User source, BaseUser destination)
    {
        destination.Decor.ShouldNotBeNull();
        destination.Decor.Count.ShouldBe(source.Decor.Count);
        for (int i = 0; i < destination.Decor.Count; i++)
        {
            UserDecor sourceDecor = source.Decor.ElementAt(i);
            BaseUserDecor destinationDecor = destination.Decor.ElementAt(i);
            destinationDecor.UserId.ShouldBe(sourceDecor.UserId);
            destinationDecor.DecorId.ShouldBe(sourceDecor.DecorId);
        }
    }

    private void VerifyUserBadges_ProjectedCorrectly(User source, BaseUser destination)
    {
        destination.Badges.ShouldNotBeNull();
        destination.Badges.Count.ShouldBe(source.Badges.Count);
        for (int i = 0; i < destination.Badges.Count; i++)
        {
            UserBadge sourceBadge = source.Badges.ElementAt(i);
            BaseUserBadge destinationBadge = destination.Badges.ElementAt(i);
            destinationBadge.UserId.ShouldBe(sourceBadge.UserId);
            destinationBadge.BadgeId.ShouldBe(sourceBadge.BadgeId);
        }
    }

    private void VerifyUserSelectedItems_ProjectedCorrectly(User source, BaseUser destination)
    {
        VerifyUserSelectedPet_ProjectedCorrectly(source, destination);
        VerifyUserSelectedIsland_ProjectedCorrectly(source, destination);
        VerifyUserSelectedDecor_ProjectedCorrectly(source, destination);
        VerifyUserSelectedBadge_ProjectedCorrectly(source, destination);
    }

    private void VerifyUserSelectedPet_ProjectedCorrectly(User source, BaseUser destination)
    {
        destination.SelectedPet.ShouldNotBeNull();
        destination.SelectedPet.Id.ShouldBe(source.SelectedPet.Id);
        destination.SelectedPet.Price.ShouldBe(source.SelectedPet.Price);
        destination.SelectedPet.Name.ShouldBe(source.SelectedPet.Name);
        destination.SelectedPet.Image.ShouldBe(source.SelectedPet.Image);
        destination.SelectedPet.HeightRequest.ShouldBe(source.SelectedPet.HeightRequest);
    }

    private void VerifyUserSelectedIsland_ProjectedCorrectly(User source, BaseUser destination)
    {
        destination.SelectedIsland.ShouldNotBeNull();
        destination.SelectedIsland.Id.ShouldBe(source.SelectedIsland.Id);
        destination.SelectedIsland.Price.ShouldBe(source.SelectedIsland.Price);
        destination.SelectedIsland.Name.ShouldBe(source.SelectedIsland.Name);
        destination.SelectedIsland.Image.ShouldBe(source.SelectedIsland.Image);
    }

    private void VerifyUserSelectedDecor_ProjectedCorrectly(User source, BaseUser destination)
    {
        destination.SelectedDecor.Id.ShouldBe(source.SelectedDecor.Id);
        destination.SelectedDecor.Price.ShouldBe(source.SelectedDecor.Price);
        destination.SelectedDecor.Name.ShouldBe(source.SelectedDecor.Name);
        destination.SelectedDecor.Image.ShouldBe(source.SelectedDecor.Image);
        destination.SelectedDecor.HeightRequest.ShouldBe(source.SelectedDecor.HeightRequest);
    }

    private void VerifyUserSelectedBadge_ProjectedCorrectly(User source, BaseUser destination)
    {
        destination.SelectedBadge.Id.ShouldBe(source.SelectedBadge.Id);
        destination.SelectedBadge.Name.ShouldBe(source.SelectedBadge.Name);
        destination.SelectedBadge.Description.ShouldBe(source.SelectedBadge.Description);
    }

    private void VerifyUserFriendships_ProjectedCorrectly(User source, BaseUser destination)
    {
        VerifyUserInvitees_ProjectedCorrectly(source, destination);
        VerifyUserInviters_ProjectedCorrectly(source, destination);
    }

    private void VerifyUserInvitees_ProjectedCorrectly(User source, BaseUser destination)
    {
        destination.Invitees.ShouldNotBeNull();
        destination.Invitees.Count.ShouldBe(source.Invitees.Count);

        Friendship sourceInviteeFriendship = source.Invitees.First();
        BaseFriendship destinationInviteeFriendship = destination.Invitees.First();
        destinationInviteeFriendship.UserId.ShouldBe(sourceInviteeFriendship.UserId);
        destinationInviteeFriendship.FriendId.ShouldNotBe(Guid.Empty);
        destinationInviteeFriendship.Status.ShouldBe(sourceInviteeFriendship.Status);
    }

    private void VerifyUserInviters_ProjectedCorrectly(User source, BaseUser destination)
    {
        destination.Inviters.ShouldNotBeNull();
        destination.Inviters.Count.ShouldBe(source.Inviters.Count);

        Friendship sourceInviterFriendship = source.Inviters.First();
        BaseFriendship destinationInviterFriendship = destination.Inviters.First();
        destinationInviterFriendship.UserId.ShouldBe(sourceInviterFriendship.UserId);
        destinationInviterFriendship.FriendId.ShouldNotBe(Guid.Empty);
        destinationInviterFriendship.Status.ShouldBe(sourceInviterFriendship.Status);
    }

    private void VerifyUserSessions_ProjectedCorrectly(User source, BaseUser destination)
    {
        destination.UserSessions.ShouldNotBeNull();
        destination.UserSessions.Count.ShouldBe(source.UserSessions.Count);
        for (int i = 0; i < source.UserSessions.Count; i++)
        {
            UserSession sourceUserSession = source.UserSessions.ElementAt(i);
            BaseUserSession destinationUserSession = destination.UserSessions.ElementAt(i);
            destinationUserSession.Id.ShouldBe(sourceUserSession.Id);
            destinationUserSession.UserId.ShouldBe(sourceUserSession.UserId);
            destinationUserSession.CurrencyEarned.ShouldBe(sourceUserSession.CurrencyEarned);
            destinationUserSession.SessionStartTime.ShouldBe(sourceUserSession.SessionStartTime);
            destinationUserSession.SessionEndTime.ShouldBe(sourceUserSession.SessionEndTime);
        }
    }

    #endregion

    #region BaseUser -> User Projection Assertions
    private void VerifyBaseUserAtomicFields_ProjectedCorrectly(BaseUser source, User destination)
    {
        destination.Id.ShouldBe(source.Id);
        destination.Auth0Id.ShouldBe(source.Auth0Id);
        destination.UserName.ShouldBe(source.UserName);
        destination.Email.ShouldBe(source.Email);
        destination.DateCreated.ShouldBe(source.DateCreated);
        destination.Balance.ShouldBe(source.Balance);
        destination.Pronouns.ShouldBe(source.Pronouns);
        destination.ProfilePicture.ShouldBe(source.ProfilePicture);
        destination.SelectedPetId.ShouldBe(source.SelectedPetId);
        destination.SelectedIslandId.ShouldBe(source.SelectedIslandId);
        destination.SelectedDecorId.ShouldBe(source.SelectedDecorId);
        destination.SelectedBadgeId.ShouldBe(source.SelectedBadgeId);
    }

    private void VerifyBaseUserPets_ProjectedCorrectly(BaseUser source, User destination)
    {
        destination.Pets.ShouldNotBeNull();
        destination.Pets.Count.ShouldBe(source.Pets.Count);
        for (int i = 0; i < destination.Pets.Count; i++)
        {
            BaseUserPet sourcePet = source.Pets.ElementAt(i);
            UserPet destinationPet = destination.Pets.ElementAt(i);
            destinationPet.UserId.ShouldBe(sourcePet.UserId);
            destinationPet.PetId.ShouldBe(sourcePet.PetId);
        }
    }

    private void VerifyBaseUserIslands_ProjectedCorrectly(BaseUser source, User destination)
    {
        destination.Islands.ShouldNotBeNull();
        destination.Islands.Count.ShouldBe(source.Islands.Count);
        for (int i = 0; i < destination.Islands.Count; i++)
        {
            BaseUserIsland sourceIsland = source.Islands.ElementAt(i);
            UserIsland destinationIsland = destination.Islands.ElementAt(i);
            destinationIsland.UserId.ShouldBe(sourceIsland.UserId);
            destinationIsland.IslandId.ShouldBe(sourceIsland.IslandId);
        }
    }

    private void VerifyBaseUserDecor_ProjectedCorrectly(BaseUser source, User destination)
    {
        destination.Decor.ShouldNotBeNull();
        destination.Decor.Count.ShouldBe(source.Decor.Count);
        for (int i = 0; i < destination.Decor.Count; i++)
        {
            BaseUserDecor sourceDecor = source.Decor.ElementAt(i);
            UserDecor destinationDecor = destination.Decor.ElementAt(i);
            destinationDecor.UserId.ShouldBe(sourceDecor.UserId);
            destinationDecor.DecorId.ShouldBe(sourceDecor.DecorId);
        }
    }

    private void VerifyBaseUserBadges_ProjectedCorrectly(BaseUser source, User destination)
    {
        destination.Badges.ShouldNotBeNull();
        destination.Badges.Count.ShouldBe(source.Badges.Count);
        for (int i = 0; i < destination.Badges.Count; i++)
        {
            BaseUserBadge sourceBadge = source.Badges.ElementAt(i);
            UserBadge destinationBadge = destination.Badges.ElementAt(i);
            destinationBadge.UserId.ShouldBe(sourceBadge.UserId);
            destinationBadge.BadgeId.ShouldBe(sourceBadge.BadgeId);
        }
    }

    private void VerifyBaseUserSelectedItems_ProjectedCorrectly(BaseUser source, User destination)
    {
        VerifyBaseUserSelectedPet_ProjectedCorrectly(source, destination);
        VerifyBaseUserSelectedIsland_ProjectedCorrectly(source, destination);
        VerifyBaseUserSelectedDecor_ProjectedCorrectly(source, destination);
        VerifyBaseUserSelectedBadge_ProjectedCorrectly(source, destination);
    }

    private void VerifyBaseUserSelectedPet_ProjectedCorrectly(BaseUser source, User destination) 
    {
        destination.SelectedPet.ShouldNotBeNull();
        destination.SelectedPet.Id.ShouldBe(source.SelectedPet.Id);
        destination.SelectedPet.Price.ShouldBe(source.SelectedPet.Price);
        destination.SelectedPet.Name.ShouldBe(source.SelectedPet.Name);
        destination.SelectedPet.Image.ShouldBe(source.SelectedPet.Image);
        destination.SelectedPet.HeightRequest.ShouldBe(source.SelectedPet.HeightRequest);
    }

    private void VerifyBaseUserSelectedIsland_ProjectedCorrectly(BaseUser source, User destination)
    {
        destination.SelectedIsland.ShouldNotBeNull();
        destination.SelectedIsland.Id.ShouldBe(source.SelectedIsland.Id);
        destination.SelectedIsland.Price.ShouldBe(source.SelectedIsland.Price);
        destination.SelectedIsland.Name.ShouldBe(source.SelectedIsland.Name);
        destination.SelectedIsland.Image.ShouldBe(source.SelectedIsland.Image);
    }

    private void VerifyBaseUserSelectedDecor_ProjectedCorrectly(BaseUser source, User destination)
    {
        destination.SelectedDecor.Id.ShouldBe(source.SelectedDecor.Id);
        destination.SelectedDecor.Price.ShouldBe(source.SelectedDecor.Price);
        destination.SelectedDecor.Name.ShouldBe(source.SelectedDecor.Name);
        destination.SelectedDecor.Image.ShouldBe(source.SelectedDecor.Image);
        destination.SelectedDecor.HeightRequest.ShouldBe(source.SelectedDecor.HeightRequest);
    }

    private void VerifyBaseUserSelectedBadge_ProjectedCorrectly(BaseUser source, User destination)
    {
        destination.SelectedBadge.Id.ShouldBe(source.SelectedBadge.Id);
        destination.SelectedBadge.Name.ShouldBe(source.SelectedBadge.Name);
        destination.SelectedBadge.Description.ShouldBe(source.SelectedBadge.Description);
    }

    private void VerifyBaseUserFriendships_ProjectedCorrectly(BaseUser source, User destination)
    {
        VerifyBaseUserInvitees_ProjectedCorrectly(source, destination);
        VerifyBaseUserInviters_ProjectedCorrectly(source, destination);
    }

    private void VerifyBaseUserInvitees_ProjectedCorrectly(BaseUser source, User destination)
    {
        destination.Invitees.ShouldNotBeNull();
        destination.Invitees.Count.ShouldBe(source.Invitees.Count);

        BaseFriendship sourceInviteeFriendship = source.Invitees.First();
        Friendship destinationInviteeFriendship = destination.Invitees.First();
        destinationInviteeFriendship.UserId.ShouldBe(sourceInviteeFriendship.UserId);
        destinationInviteeFriendship.FriendId.ShouldNotBe(Guid.Empty);
        destinationInviteeFriendship.Status.ShouldBe(sourceInviteeFriendship.Status);
    }

    private void VerifyBaseUserInviters_ProjectedCorrectly(BaseUser source, User destination)
    {
        destination.Inviters.ShouldNotBeNull();
        destination.Inviters.Count.ShouldBe(source.Inviters.Count);

        BaseFriendship sourceInviterFriendship = source.Inviters.First();
        Friendship destinationInviterFriendship = destination.Inviters.First();
        destinationInviterFriendship.UserId.ShouldBe(sourceInviterFriendship.UserId);
        destinationInviterFriendship.FriendId.ShouldNotBe(Guid.Empty);
        destinationInviterFriendship.Status.ShouldBe(sourceInviterFriendship.Status);
    }

    private void VerifyBaseUserSessions_ProjectedCorrectly(BaseUser source, User destination)
    {
        destination.UserSessions.ShouldNotBeNull();
        destination.UserSessions.Count.ShouldBe(source.UserSessions.Count);
        for (int i = 0; i < source.UserSessions.Count; i++)
        {
            BaseUserSession sourceUserSession = source.UserSessions.ElementAt(i);
            UserSession destinationUserSession = destination.UserSessions.ElementAt(i);
            destinationUserSession.Id.ShouldBe(sourceUserSession.Id);
            destinationUserSession.UserId.ShouldBe(sourceUserSession.UserId);
            destinationUserSession.CurrencyEarned.ShouldBe(sourceUserSession.CurrencyEarned);
            destinationUserSession.SessionStartTime.ShouldBe(sourceUserSession.SessionStartTime);
            destinationUserSession.SessionEndTime.ShouldBe(sourceUserSession.SessionEndTime);
        }
    }

    #endregion
}