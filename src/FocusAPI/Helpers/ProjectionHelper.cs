using FocusAPI.Models;
using FocusCore.Models;

namespace FocusAPI.Helpers;

public static class ProjectionHelper
{
    public static BaseUser ProjectToBaseUser(User user) =>
        new()
        {
            Id = user.Id,
            Auth0Id = user.Auth0Id,
            UserName = user.UserName,
            Email = user.Email,
            DateCreated = user.DateCreated,
            Balance = user.Balance,
            Pronouns = user.Pronouns,
            ProfilePicture = user.ProfilePicture,
            Inviters = user.Inviters.Select(i => i as BaseFriendship).ToList(),
            Invitees = user.Invitees.Select(i => i as BaseFriendship).ToList(),
            Pets = user.Pets.Select(pet => pet as BaseUserPet).ToList(),
            Decor = user.Decor.Select(decor => decor as BaseUserDecor).ToList(),
            Badges = user.Badges.Select(badge => badge as BaseUserBadge).ToList(),
            Islands = user.Islands.Select(island => island as BaseUserIsland).ToList(),
            UserSessions = user.UserSessions.Select(userSession => userSession as BaseUserSession).ToList(),
            SelectedIslandId = user.SelectedIslandId,
            SelectedIsland = user.SelectedIsland,
            SelectedPetId = user.SelectedPetId,
            SelectedPet = user.SelectedPet,
            SelectedDecorId = user.SelectedDecorId,
            SelectedDecor = user.SelectedDecor,
            SelectedBadgeId = user.SelectedBadgeId,
            SelectedBadge = user.SelectedBadge
        };

    public static User ProjectFromBaseUser(BaseUser user) =>
        new()
        {
            Id = user.Id,
            Auth0Id = user.Auth0Id,
            UserName = user.UserName,
            Email = user.Email,
            DateCreated = user.DateCreated.UtcDateTime,
            Balance = user.Balance,
            Pronouns = user.Pronouns,
            ProfilePicture = user.ProfilePicture,
            Inviters = user.Inviters.Select(i => i as Friendship).ToList(),
            Invitees = user.Invitees.Select(i => i as Friendship).ToList(),
            Pets = user.Pets != null ? user.Pets.Select(ProjectFromBaseUserPet).ToList() : null,
            Decor = user.Decor != null ? user.Decor.Select(ProjectFromBaseUserDecor).ToList() : null,
            Badges = user.Badges != null ? user.Badges.Select(ProjectFromBaseUserBadge).ToList() : null,
            Islands = user.Islands != null ? user.Islands.Select(ProjectFromBaseUserIsland).ToList() : null,
            UserSessions = user.UserSessions.Select(userSession => userSession as UserSession).ToList(),
            SelectedIslandId = user.SelectedIslandId,
            SelectedIsland = user.SelectedIsland != null ? ProjectFromBaseIsland(user.SelectedIsland) : null,
            SelectedPetId = user.SelectedPetId,
            SelectedPet = user.SelectedPet != null ? ProjectFromBasePet(user.SelectedPet) : null,
            SelectedDecorId = user.SelectedDecorId,
            SelectedDecor = user.SelectedDecor != null ? ProjectFromBaseDecor(user.SelectedDecor) : null,
            SelectedBadgeId = user.SelectedBadgeId,
            SelectedBadge = user.SelectedBadge != null ? ProjectFromBaseBadge(user.SelectedBadge) : null
        };


    public static BasePet ProjectToBasePet(Pet pet) =>
        new BasePet
        {
            Id = pet.Id,
            Name = pet.Name,
            Image = pet.Image,
            Price = pet.Price,
            HeightRequest = pet.HeightRequest
        };

    public static Pet ProjectFromBasePet(BasePet pet) =>
        new Pet
        {
            Id = pet.Id,
            Name = pet.Name,
            Image = pet.Image,
            Price = pet.Price,
            HeightRequest = pet.HeightRequest
        };

    public static BaseIsland ProjectToBaseIsland(Island island) =>
        new BaseIsland
        {
            Id = island.Id,
            Name = island.Name,
            Image = island.Image,
            Price = island.Price
        };

    public static Island ProjectFromBaseIsland(BaseIsland island) =>
        new Island
        {
            Id = island.Id,
            Name = island.Name,
            Image = island.Image,
            Price = island.Price
        };

    public static BaseMindfulnessTip ProjectToBaseMindfulnessTip(MindfulnessTip tip) =>
        new()
        {
            Id = tip.Id,
            Content = tip.Content,
            SessionRatingLevel = tip.SessionRatingLevel,
            Title = tip.Title
        };

    public static BaseBadge ProjectToBaseBadge(Badge badge) =>
        new()
        {
            Id = badge.Id,
            Name = badge.Name,
            Image = badge.Image,
            Description = badge.Description
        };

    public static Badge ProjectFromBaseBadge(BaseBadge badge) =>
        new()
        {
            Id = badge.Id,
            Name = badge.Name,
            Image = badge.Image,
            Description = badge.Description
        };

    public static BaseDecor ProjectToBaseDecor(Decor decor) =>
        new()
        {
            Id = decor.Id,
            Name = decor.Name,
            Image = decor.Image,
            Price = decor.Price,
            HeightRequest = decor.HeightRequest
        };

    public static Decor ProjectFromBaseDecor(BaseDecor decor) =>
        new()
        {
            Id = decor.Id,
            Name = decor.Name,
            Image = decor.Image,
            Price = decor.Price,
            HeightRequest = decor.HeightRequest
        };

    public static UserPet ProjectFromBaseUserPet(BaseUserPet userPet) =>
        new UserPet
        {
            UserId = userPet.UserId,
            PetId = userPet.PetId,
            DateAcquired = userPet.DateAcquired
        };

    public static UserDecor ProjectFromBaseUserDecor(BaseUserDecor userDecor) =>
        new UserDecor
        {
            UserId = userDecor.UserId,
            DecorId = userDecor.DecorId,
            DateAcquired = userDecor.DateAcquired
        };

    public static UserBadge ProjectFromBaseUserBadge(BaseUserBadge userBadge) =>
        new UserBadge
        {
            UserId = userBadge.UserId,
            BadgeId = userBadge.BadgeId,
            DateAcquired = userBadge.DateAcquired
        };

    public static UserIsland ProjectFromBaseUserIsland(BaseUserIsland userIsland) =>
        new UserIsland
        {
            UserId = userIsland.UserId,
            IslandId = userIsland.IslandId,
            DateAcquired = userIsland.DateAcquired
        };
}

