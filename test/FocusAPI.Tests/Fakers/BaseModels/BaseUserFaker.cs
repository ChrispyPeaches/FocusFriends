using Bogus;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels;
internal class BaseUserFaker : Faker<BaseUser>
{
    internal BaseUserFaker()
    {
        Guid userId = Guid.NewGuid();
        RuleFor(user => user.Id, f => userId);
        RuleFor(user => user.Auth0Id, f => f.Random.Word());

        BaseUserPetFaker userPetFaker = new(userId);
        RuleFor(user => user.Pets, f => userPetFaker.Generate(2));

        BaseUserIslandFaker userIslandFaker = new(userId);
        RuleFor(user => user.Islands, f => userIslandFaker.Generate(2));

        BaseUserDecorFaker userDecorFaker = new(userId);
        RuleFor(user => user.Decor, f => userDecorFaker.Generate(2));

        BaseUserBadgeFaker userBadgeFaker = new(userId);
        RuleFor(user => user.Badges, f => userBadgeFaker.Generate(2));
    }
}