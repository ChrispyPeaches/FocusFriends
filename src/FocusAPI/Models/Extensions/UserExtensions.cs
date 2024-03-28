﻿using FocusCore.Models;

namespace FocusAPI.Models.Extensions;

public static class UserExtensions
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
            FirstName = user.FirstName,
            LastName = user.LastName,
            Pronouns = user.Pronouns,
            ProfilePicture = user.ProfilePicture,
            Inviters = user.Inviters.Select(i => i as BaseFriendship).ToList(),
            Invitees = user.Invitees.Select(i => i as BaseFriendship).ToList(),
            Pets = user.Pets.Select(pet => pet as BaseUserPet).ToList(),
            Furniture = user.Furniture.Select(furniture => furniture as BaseUserFurniture).ToList(),
            Sounds = user.Sounds.Select(sound => sound as BaseUserSound).ToList(),
            Badges = user.Badges.Select(badge => badge as BaseUserBadge).ToList(),
            Islands = user.Islands.Select(island => island as BaseUserIsland).ToList(),
            UserSessions = user.UserSessions.Select(userSession => userSession as BaseUserSession).ToList(),
            SelectedIslandId = user.SelectedIslandId,
            SelectedIsland = user.SelectedIsland,
            SelectedPetId = user.SelectedPetId,
            SelectedPet = user.SelectedPet,
            SelectedFurnitureId = user.SelectedFurnitureId,
            SelectedFurniture = user.SelectedFurniture,
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
            DateCreated = user.DateCreated,
            Balance = user.Balance,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Pronouns = user.Pronouns,
            ProfilePicture = user.ProfilePicture,
            Inviters = user.Inviters.Select(i => i as Friendship).ToList(),
            Invitees = user.Invitees.Select(i => i as Friendship).ToList(),
            Pets = user.Pets.Select(pet => pet as UserPet).ToList(),
            Furniture = user.Furniture.Select(furniture => furniture as UserFurniture).ToList(),
            Sounds = user.Sounds.Select(sound => sound as UserSound).ToList(),
            Badges = user.Badges.Select(badge => badge as UserBadge).ToList(),
            Islands = user.Islands.Select(island => island as UserIsland).ToList(),
            UserSessions = user.UserSessions.Select(userSession => userSession as UserSession).ToList(),
            SelectedIslandId = user.SelectedIslandId,
            SelectedIsland = user.SelectedIsland as Island,
            SelectedPetId = user.SelectedPetId,
            SelectedPet = user.SelectedPet as Pet,
            SelectedFurnitureId = user.SelectedFurnitureId,
            SelectedFurniture = user.SelectedFurniture as Furniture,
            SelectedBadgeId = user.SelectedBadgeId,
            SelectedBadge = user.SelectedBadge as Badge
        };
}

