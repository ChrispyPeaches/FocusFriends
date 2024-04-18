using FocusApp.Shared.Models;
using FocusCore.Models;

namespace FocusApp.Client.Helpers;

public static class ProjectionHelper
{
    public static BaseUser ProjectToBaseUser(User user) =>
        new()
        {
            Id = user.Id,
            Auth0Id = user.Auth0Id,
            UserName = user.UserName,
            Email = user.Email,
            DateCreated = new DateTimeOffset(user.DateCreated, TimeZoneInfo.Utc.GetUtcOffset(user.DateCreated)),
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
            Pets = user.Pets.Select(up => new UserPet { UserId = user.Id, PetId = up.PetId }).ToList(),
            Decor = user.Decor.Select(uf => new UserDecor { UserId = user.Id, DecorId = uf.DecorId }).ToList(),
            Badges = user.Badges.Select(ub => new UserBadge { UserId = user.Id, BadgeId = ub.BadgeId }).ToList(),
            Islands = user.Islands.Select(ui => new UserIsland { UserId = user.Id, IslandId = ui.IslandId }).ToList(),
            UserSessions = user.UserSessions.Select(userSession => userSession as UserSession).ToList(),
            SelectedIslandId = user.SelectedIslandId,
            SelectedIsland = user.SelectedIsland as Island,
            SelectedPetId = user.SelectedPetId,
            SelectedPet = user.SelectedPet as Pet,
            SelectedDecorId = user.SelectedDecorId,
            SelectedDecor = user.SelectedDecor as Decor,
            SelectedBadgeId = user.SelectedBadgeId,
            SelectedBadge = user.SelectedBadge as Badge
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

    public static MindfulnessTip ProjectFromBaseMindfulnessTip(BaseMindfulnessTip tip) =>
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
            Image = badge.Image
        };

    public static Badge ProjectFromBaseBadge(BaseBadge badge) =>
        new()
        {
            Id = badge.Id,
            Name = badge.Name,
            Image = badge.Image
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
}

