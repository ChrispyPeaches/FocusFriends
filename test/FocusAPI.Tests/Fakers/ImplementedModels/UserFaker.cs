using Bogus;
using FocusAPI.Models;

namespace FocusAPI.Tests.Fakers.ImplementedModels;
internal class UserFaker : Faker<User>
{
    internal UserFaker()
    {
        Guid userId = Guid.NewGuid();
        RuleFor(user => user.Id, f => userId);
        RuleFor(user => user.Auth0Id, f => f.Random.Word());
        RuleFor(user => user.UserName, f => f.Random.Word());
        RuleFor(user => user.Email, f => f.Internet.Email());
        RuleFor(user => user.DateCreated, f => f.Date.Recent());
        RuleFor(user => user.Balance, f => Math.Abs(f.Random.Int()));
        RuleFor(user => user.Pronouns, f => f.Random.Word());
        RuleFor(user => user.ProfilePicture, f => [0x1]);

        UserPetFaker userPetFaker = new(userId);
        List<UserPet> userPets = userPetFaker.Generate(2);
        RuleFor(user => user.Pets, f => userPets);
        RuleFor(user => user.SelectedPet, f => userPets.First().Pet);
        RuleFor(user => user.SelectedPetId, f => userPets.First().PetId);

        UserIslandFaker userIslandFaker = new(userId);
        List<UserIsland> userIslands = userIslandFaker.Generate(2);
        RuleFor(user => user.Islands, f => userIslands);
        RuleFor(user => user.SelectedIsland, f => userIslands.First().Island);
        RuleFor(user => user.SelectedIslandId, f => userIslands.First().IslandId);
        
        UserDecorFaker userDecorFaker = new(userId);
        List<UserDecor> userDecor = userDecorFaker.Generate(2);
        RuleFor(user => user.Decor, f => userDecor);
        RuleFor(user => user.SelectedDecor, f => userDecor.First().Decor);
        RuleFor(user => user.SelectedDecorId, f => userDecor.First().DecorId);

        UserBadgeFaker userBadgeFaker = new(userId);
        List<UserBadge> userBadges = userBadgeFaker.Generate(2);
        RuleFor(user => user.Badges, f => userBadges);
        RuleFor(user => user.SelectedBadge, f => userBadges.First().Badge);
        RuleFor(user => user.SelectedBadgeId, f => userBadges.First().BadgeId);
    }
}