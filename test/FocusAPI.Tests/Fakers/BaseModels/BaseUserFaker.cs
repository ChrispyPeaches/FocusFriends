using Bogus;
using FocusAPI.Tests.Fakers.ImplementedModels;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels;
internal class BaseUserFaker : Faker<BaseUser>
{
    internal BaseUserFaker()
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

        BaseUserPetFaker baseUserPetFaker = new(userId);
        List<BaseUserPet> baseUserPets = baseUserPetFaker.Generate(2);
        RuleFor(user => user.Pets, f => baseUserPets);
        RuleFor(user => user.SelectedPet, f => baseUserPets.First().Pet);
        RuleFor(user => user.SelectedPetId, f => baseUserPets.First().PetId);

        BaseUserIslandFaker baseUserIslandFaker = new(userId);
        List<BaseUserIsland> baseUserIslands = baseUserIslandFaker.Generate(2);
        RuleFor(user => user.Islands, f => baseUserIslands);
        RuleFor(user => user.SelectedIsland, f => baseUserIslands.First().Island);
        RuleFor(user => user.SelectedIslandId, f => baseUserIslands.First().IslandId);

        BaseUserDecorFaker baseUserDecorFaker = new(userId);
        List<BaseUserDecor> baseUserDecor = baseUserDecorFaker.Generate(2);
        RuleFor(user => user.Decor, f => baseUserDecor);
        RuleFor(user => user.SelectedDecor, f => baseUserDecor.First().Decor);
        RuleFor(user => user.SelectedDecorId, f => baseUserDecor.First().DecorId);

        BaseUserBadgeFaker baseUserBadgeFaker = new(userId);
        List<BaseUserBadge> baseUserBadges = baseUserBadgeFaker.Generate(2);
        RuleFor(user => user.Badges, f => baseUserBadges);
        RuleFor(user => user.SelectedBadge, f => baseUserBadges.First().Badge);
        RuleFor(user => user.SelectedBadgeId, f => baseUserBadges.First().BadgeId);
    }
}